using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

public static class Utills
{

    public static void Reflect(object reflectedObject, BindingFlags bindingFlags, string prefix = "")
    {
        Type type = reflectedObject.GetType();
        if (prefix == string.Empty)
        {
            prefix += reflectedObject.GetType();
        }
        PropertyInfo[] prpertyInfomation = type.GetProperties(bindingFlags);
        for (int i = 0; i < prpertyInfomation.Length; i++)
        {
            string rwPermitions = "";
            if (prpertyInfomation[i].CanRead && prpertyInfomation[i].CanWrite)
            {
                rwPermitions = "rw";
            }
            else if (prpertyInfomation[i].CanRead && !prpertyInfomation[i].CanWrite)
            {
                rwPermitions = "ro";
            }
            else if (!prpertyInfomation[i].CanRead && prpertyInfomation[i].CanWrite)
            {
                rwPermitions = "wo";
            }
            else if (!prpertyInfomation[i].CanRead && !prpertyInfomation[i].CanWrite)
            {
                rwPermitions = "none";
            }
            Debug.Log($"<b>{prefix}:</b> {prpertyInfomation[i].Name}, type: {prpertyInfomation[i].PropertyType}, value: {prpertyInfomation[i].GetValue(reflectedObject)}, readWritePermitions: {rwPermitions}, GetMethod: {prpertyInfomation[i].GetMethod}, SetMethod: {prpertyInfomation[i].SetMethod}");
            if (DoesNeedReflection(prpertyInfomation[i].PropertyType))
            {
                prefix += "->" + prpertyInfomation[i].Name;
                if (prpertyInfomation[i].GetValue(reflectedObject) == null)
                {
                    Debug.LogWarning($"<b>{prefix}:</b> {prpertyInfomation[i].Name} Is set to null. The branch will be stoped.");
                    continue;
                }
                Reflect(prpertyInfomation[i].GetValue(reflectedObject), bindingFlags, prefix);
            }
        }
        FieldInfo[] fieldInfomation = type.GetFields(bindingFlags);
        for (int i = 0; i < fieldInfomation.Length; i++)
        {
            Debug.Log($"<b>{prefix}:</b> {fieldInfomation[i].Name}, type: {fieldInfomation[i].FieldType}, value: {fieldInfomation[i].GetValue(reflectedObject)}, isPublic: {fieldInfomation[i].IsPublic}, isPrivate: {fieldInfomation[i].IsPrivate}, isStatic: {fieldInfomation[i].IsStatic}");
            if (DoesNeedReflection(fieldInfomation[i].FieldType))
            {
                prefix += "->" + fieldInfomation[i].Name;
                if (fieldInfomation[i].GetValue(reflectedObject) == null)
                {
                    Debug.LogWarning($"<b>{prefix}:</b> {fieldInfomation[i].Name} Is set to null. The branch will be stoped.");
                    continue;
                }
                Reflect(fieldInfomation[i].GetValue(reflectedObject), bindingFlags, prefix);
            }
        }


        bool DoesNeedReflection(Type testedType)
        {
            if (TestVsNormalTypes() || IsTypeIEnumeratbleType())
            {
                if (testedType == type)
                {
                    Debug.LogWarning($"<b>{prefix}:</b> {testedType} has a veriable of same type. The branch will be stoped");
                }
                return false;
            }
            return true;


            bool TestVsNormalTypes()
            {
                return testedType == typeof(byte) || testedType == typeof(short) || testedType == typeof(int) || testedType == typeof(long) || testedType == typeof(IntPtr) || testedType == typeof(sbyte) || testedType == typeof(ushort) || testedType == typeof(uint) || testedType == typeof(ulong) | testedType == typeof(UIntPtr) || testedType == typeof(float) || testedType == typeof(double) || testedType == typeof(decimal) || testedType == typeof(bool) || testedType == typeof(char) || testedType == typeof(string) || testedType == typeof(Type) || testedType == typeof(Assembly) || testedType == type;
            }
            bool IsTypeIEnumeratbleType()
            {
                return DoesContainInterface(typeof(IEnumerable<byte>)) || DoesContainInterface(typeof(IEnumerable<short>)) || DoesContainInterface(typeof(IEnumerable<int>)) || DoesContainInterface(typeof(IEnumerable<long>)) || DoesContainInterface(typeof(IEnumerable<IntPtr>)) || DoesContainInterface(typeof(IEnumerable<sbyte>)) || DoesContainInterface(typeof(IEnumerable<ushort>)) || DoesContainInterface(typeof(IEnumerable<uint>)) || DoesContainInterface(typeof(IEnumerable<ulong>)) || DoesContainInterface(typeof(IEnumerable<UIntPtr>)) || DoesContainInterface(typeof(IEnumerable<float>)) || DoesContainInterface(typeof(IEnumerable<double>)) || DoesContainInterface(typeof(IEnumerable<decimal>)) || DoesContainInterface(typeof(IEnumerable<bool>)) || DoesContainInterface(typeof(IEnumerable<char>)) || DoesContainInterface(typeof(IEnumerable<string>)) || DoesContainInterface(typeof(IEnumerable<Type>)) || DoesContainInterface(typeof(IEnumerable<Assembly>));
            }
            bool DoesContainInterface(Type interfaceType)
            {
                return testedType.GetInterfaces().Contains(interfaceType);
            }
        }
    }
    
    public static void ReflectType(Type type, BindingFlags bindingFlags, string prefix = "", int j = 0)
    {
        if (j > 5)
        {
            Debug.Log("STOP!");
            return;
        }

        if (prefix == string.Empty)
        {
            prefix += type;
        }
        PropertyInfo[] prpertyInfomation = type.GetProperties(bindingFlags);
        for (int i = 0; i < prpertyInfomation.Length; i++)
        {
            string rwPermitions = "";
            if (prpertyInfomation[i].CanRead && prpertyInfomation[i].CanWrite)
            {
                rwPermitions = "rw";
            }
            else if (prpertyInfomation[i].CanRead && !prpertyInfomation[i].CanWrite)
            {
                rwPermitions = "ro";
            }
            else if (!prpertyInfomation[i].CanRead && prpertyInfomation[i].CanWrite)
            {
                rwPermitions = "wo";
            }
            else if (!prpertyInfomation[i].CanRead && !prpertyInfomation[i].CanWrite)
            {
                rwPermitions = "none";
            }
            Debug.Log($"{j}<b>{prefix}:</b> {prpertyInfomation[i].Name}, type: {prpertyInfomation[i].PropertyType}, readWritePermitions: {rwPermitions}, GetMethod: {prpertyInfomation[i].GetMethod}, SetMethod: {prpertyInfomation[i].SetMethod}");
            if (DoesNeedReflection(prpertyInfomation[i].PropertyType) && j < 2)
            {
                prefix += "->" + prpertyInfomation[i].Name;
                j++;
                ReflectType(prpertyInfomation[i].PropertyType, bindingFlags, prefix, j);
            }
        }
        FieldInfo[] fieldInfomation = type.GetFields(bindingFlags);
        for (int i = 0; i < fieldInfomation.Length; i++)
        {
            Debug.Log($"{j}<b>{prefix}:</b> {fieldInfomation[i].Name}, type: {fieldInfomation[i].FieldType}, isPublic: {fieldInfomation[i].IsPublic}, isPrivate: {fieldInfomation[i].IsPrivate}, isStatic: {fieldInfomation[i].IsStatic}");
            if (DoesNeedReflection(fieldInfomation[i].FieldType) && j < 2)
            {
                prefix += "->" + fieldInfomation[i].Name;
                j++;
                ReflectType(fieldInfomation[i].FieldType, bindingFlags, prefix, j);
            }
        }


        bool DoesNeedReflection(Type testedType)
        {
            if (TestVsNormalTypes() || IsTypeIEnumeratbleType())
            {
                if (testedType == type)
                {
                    Debug.LogWarning($"<b>{prefix}:</b> {testedType} has a veriable of same type. The branch will be stoped");
                }
                return false;
            }
            return true;


            bool TestVsNormalTypes()
            {
                return testedType == typeof(byte) || testedType == typeof(short) || testedType == typeof(int) || testedType == typeof(long) || testedType == typeof(IntPtr) || testedType == typeof(sbyte) || testedType == typeof(ushort) || testedType == typeof(uint) || testedType == typeof(ulong) | testedType == typeof(UIntPtr) || testedType == typeof(float) || testedType == typeof(double) || testedType == typeof(decimal) || testedType == typeof(bool) || testedType == typeof(char) || testedType == typeof(string) || testedType == typeof(Type) || testedType == typeof(Assembly) || testedType == type;
            }
            bool IsTypeIEnumeratbleType()
            {
                return DoesContainInterface(typeof(IEnumerable<byte>)) || DoesContainInterface(typeof(IEnumerable<short>)) || DoesContainInterface(typeof(IEnumerable<int>)) || DoesContainInterface(typeof(IEnumerable<long>)) || DoesContainInterface(typeof(IEnumerable<IntPtr>)) || DoesContainInterface(typeof(IEnumerable<sbyte>)) || DoesContainInterface(typeof(IEnumerable<ushort>)) || DoesContainInterface(typeof(IEnumerable<uint>)) || DoesContainInterface(typeof(IEnumerable<ulong>)) || DoesContainInterface(typeof(IEnumerable<UIntPtr>)) || DoesContainInterface(typeof(IEnumerable<float>)) || DoesContainInterface(typeof(IEnumerable<double>)) || DoesContainInterface(typeof(IEnumerable<decimal>)) || DoesContainInterface(typeof(IEnumerable<bool>)) || DoesContainInterface(typeof(IEnumerable<char>)) || DoesContainInterface(typeof(IEnumerable<string>)) || DoesContainInterface(typeof(IEnumerable<Type>)) || DoesContainInterface(typeof(IEnumerable<Assembly>));
            }
            bool DoesContainInterface(Type interfaceType)
            {
                return testedType.GetInterfaces().Contains(interfaceType);
            }
        }
    }

}
