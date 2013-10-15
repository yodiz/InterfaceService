using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InterfaceService
{

    public interface IMethodCaller
    {
        object Call(string methodName, string[] parameterNames, object[] parameterValues, Type[] parameterTypes, Type expectedReturn);
    }


    //TODO: Remove this
    public class Caller
    {
        public static object CallMethod(IMethodCaller caller, string methodName, string[] parameterNames, object[] parameterValues, Type[] parameterTypes, Type expectedReturn)
        {
            return caller.Call(methodName, parameterNames, parameterValues, parameterTypes, expectedReturn);
        }
    }

    public class ProxyInterface<T>
    {
        public ProxyInterface(IMethodCaller caller)
        {
            var assemblyName = new AssemblyName { Name = "ProxyInterfaceGenerated" };
            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var modBuild = assemblyBuilder.DefineDynamicModule("ProxyInterfaceGenerated");

            var typeBuilder = modBuild.DefineType("Generated"+typeof(T).Name,
                          TypeAttributes.Public |
                          TypeAttributes.Class |
                          TypeAttributes.AutoLayout |
                          TypeAttributes.AnsiClass |
                          TypeAttributes.BeforeFieldInit
                          );

            var ccField = typeBuilder.DefineField("_cc", typeof(IMethodCaller), FieldAttributes.Private);

            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(IMethodCaller) });
            var cil = constructor.GetILGenerator();
            cil.Emit(OpCodes.Ldarg_0);
            cil.Emit(OpCodes.Ldarg_1);
            cil.Emit(OpCodes.Stfld, ccField);
            cil.Emit(OpCodes.Ret);

            var interfaceType = typeof(T);
            typeBuilder.AddInterfaceImplementation(typeof(T));

            foreach (var interfaceMethod in interfaceType.GetMethods())
            {
                var methodBuilder = typeBuilder.DefineMethod(interfaceMethod.Name, MethodAttributes.Public |
                                    MethodAttributes.Virtual |
                                    MethodAttributes.Final |
                                    MethodAttributes.NewSlot |
                                    MethodAttributes.HideBySig);
                methodBuilder.SetReturnType(interfaceMethod.ReturnType);
                var parameters = interfaceMethod
                    .GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .ToArray();
                var parameterName = interfaceMethod
                    .GetParameters()
                    .Select(parameter => parameter.Name)
                    .ToArray();
                methodBuilder.SetParameters(parameters);
                typeBuilder.DefineMethodOverride(methodBuilder, interfaceMethod);

                var il = methodBuilder.GetILGenerator();
                var label = il.DefineLabel();
                il.DeclareLocal(typeof(int));

                var cally = typeof(Caller).GetMethod("CallMethod");

                LocalBuilder parmeterValues = il.DeclareLocal(typeof(object[]));
                LocalBuilder parameterNames = il.DeclareLocal(typeof(string[]));
                LocalBuilder parameterTypes = il.DeclareLocal(typeof(Type[]));
                LocalBuilder localValue = il.DeclareLocal(typeof(object));


                il.Emit(OpCodes.Nop); //Not needed, for breakpoints etc.

                il.Emit(OpCodes.Ldc_I4, parameters.Length); //length of array to create
                il.Emit(OpCodes.Newarr, typeof(object)); // create array of type with length of current value on the stack
                il.Emit(OpCodes.Stloc, parmeterValues); //Store thing on stack to local (variable?)

                il.Emit(OpCodes.Ldc_I4, parameters.Length); //length of array to create
                il.Emit(OpCodes.Newarr, typeof(string));
                il.Emit(OpCodes.Stloc, parameterNames);

                il.Emit(OpCodes.Ldc_I4, parameters.Length); //length of array to create
                il.Emit(OpCodes.Newarr, typeof(Type));
                il.Emit(OpCodes.Stloc, parameterTypes); //Store thing on stack to local (variable?)


                for (int i = 0; i < parameters.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc, parmeterValues); //Put our array on stack
                    il.Emit(OpCodes.Ldc_I4, i); //Put value on stack indicating which fiueld we wanna work on
                    il.Emit(OpCodes.Ldarg, i + 1); //Load argument at position on stack
                    il.Emit(OpCodes.Box, parameters[i]); //Box value at stack
                    il.Emit(OpCodes.Stelem_Ref); //Replace element in array with given value

                    il.Emit(OpCodes.Ldloc, parameterNames); //Put our array on stack
                    il.Emit(OpCodes.Ldc_I4, i); //Put value on stack indicating which fiueld we wanna work on
                    il.Emit(OpCodes.Ldstr, parameterName[i]);
                    il.Emit(OpCodes.Stelem_Ref); //Replace element in array with given value

                    il.Emit(OpCodes.Ldloc, parameterTypes); //Put our array on stack
                    il.Emit(OpCodes.Ldc_I4, i); //Put value on stack indicating which fiueld we wanna work on
                    il.Emit(OpCodes.Ldtoken, parameters[i]);
                    il.Emit(OpCodes.Stelem_Ref); //Replace element in array with given value
                }


                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, ccField);
                il.Emit(OpCodes.Ldstr, interfaceMethod.Name); //Put a string on the stack
                il.Emit(OpCodes.Ldloc, parameterNames); //Put arrray at stack
                il.Emit(OpCodes.Ldloc, parmeterValues); //Put arrray at stack
                il.Emit(OpCodes.Ldloc, parameterTypes); //Put arrray at stack
                il.Emit(OpCodes.Ldtoken, interfaceMethod.ReturnType);

                il.Emit(OpCodes.Call, cally); //Call method
                //il.Emit(OpCodes.Box, typeof(object)); //Box value at stack
                il.Emit(OpCodes.Stloc, localValue);

                if (interfaceMethod.ReturnType != typeof(void))
                {

                    il.Emit(OpCodes.Ldloc, localValue);
                    il.Emit(OpCodes.Unbox_Any, interfaceMethod.ReturnType);

                    //il.Emit(OpCodes.Ldc_I4, 0);
                }

                il.Emit(OpCodes.Ret);
            }

            var type = typeBuilder.CreateType();
            this.Client = (T)Activator.CreateInstance(type, caller);
        }

        public readonly T Client;
    }
}
