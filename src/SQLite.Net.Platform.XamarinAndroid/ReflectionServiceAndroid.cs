using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SQLite.Net.Interop;

namespace SQLite.Net.Platform.XamarinAndroid
{
    public class ReflectionServiceAndroid : IReflectionService
    {
        public IEnumerable<PropertyInfo> GetPublicInstanceProperties(Type mappedType)
        {
            return mappedType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public object GetMemberValue(object obj, Expression expr, MemberInfo member)
        {
            var propertyInfo = member as PropertyInfo;
            var fieldInfo = member as FieldInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj, null);
            }

            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }

            throw new NotSupportedException("MemberExpr: " + member.GetType().FullName);
        }
    }
}