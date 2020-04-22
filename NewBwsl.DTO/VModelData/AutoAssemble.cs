using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewMK.DTO
{
    public class SelectField : Attribute
    {
        /// <summary>
        /// 条件类型（默认：and,or）
        /// </summary>
        public string ConditionType { get; set; }

        /// <summary>
        /// 条件值（=，！=，>，<，>=，<=，in,on）
        /// </summary>
        public string ConditionValue { get; set; }

        /// <summary>
        /// 值类型
        /// </summary>
        public string ValueType { get; set; }

        public SelectField(string conditionType, string conditionValue, string valueType)
        {
            ConditionType = conditionType;
            ConditionValue = conditionValue;
            ValueType = valueType;
        }
    }

    public class AutoAssemble
    {
        public static Expression<Func<Model, bool>> Splice<Model, Request_DTO>(Request_DTO request_Order)
        {
            Expression<Func<Model, bool>> condition = n => true;
            IEnumerable<System.Reflection.PropertyInfo> sss = request_Order.GetType().GetProperties();
            foreach (var item in sss)
            {
                SelectField field = item.GetCustomAttribute(typeof(SelectField)) as SelectField;


                if (field != null)
                {
                    string key = item.Name;
                    object value = item.GetValue(request_Order, null);

                    string valueType = field.ValueType;

                    bool isValueNull = false;

                    if (value != null)
                    {
                        isValueNull = true;
                    }


                    if (isValueNull)
                    {
                        string conditionValue = field.ConditionValue;
                        Expression<Func<Model, bool>> tempCondition = CreateLambda<Model>(key, value, conditionValue);
                        if (field.ConditionType.Equals("or"))
                        {
                            condition = condition.Or2(tempCondition);
                        }
                        else
                        {
                            condition = condition.And2(tempCondition);
                        }
                    }
                }
            }


            return condition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">字段</param>
        /// <param name="value">值</param>
        /// <param name="conditionValue">条件表达式（=，！=，>，<，>=，<=，in,on）</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateLambda<T>(string key, object value, string conditionValue)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            var constant = Expression.Constant(value);
            MemberExpression member = Expression.PropertyOrField(parameter, key);
            if ("=".Equals(conditionValue))
            {
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(member, constant), parameter);
            }
            else if ("!=".Equals(conditionValue))
            {
                return Expression.Lambda<Func<T, bool>>(Expression.NotEqual(member, constant), parameter);
            }
            else if (">".Equals(conditionValue))
            {
                return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(member, constant), parameter);
            }
            else if ("<".Equals(conditionValue))
            {
                return Expression.Lambda<Func<T, bool>>(Expression.LessThan(member, constant), parameter);
            }
            else if (">=".Equals(conditionValue))
            {
                return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(member, constant), parameter);
            }
            else if ("<=".Equals(conditionValue))
            {
                return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(member, constant), parameter);
            }
            else if ("in".Equals(conditionValue))
            {
                return GetExpressionWithMethod<T>("Contains", key, value);
            }
            else if ("out".Equals(conditionValue))
            {
                return GetExpressionWithoutMethod<T>("Contains", key, value);
            }
            else
            {
                return null;
            }
        }
        public static Expression<Func<T, bool>> GetExpressionWithMethod<T>(string methodName, string key, object value)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "p");
            MethodCallExpression methodExpression = GetMethodExpression(methodName, key, value, parameterExpression);
            return Expression.Lambda<Func<T, bool>>(methodExpression, parameterExpression);
        }

        public static Expression<Func<T, bool>> GetExpressionWithoutMethod<T>(string methodName, string key, object value)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "p");
            MethodCallExpression methodExpression = GetMethodExpression(methodName, key, value, parameterExpression);
            var notMethodExpression = Expression.Not(methodExpression);
            return Expression.Lambda<Func<T, bool>>(notMethodExpression, parameterExpression);
        }
        /// <summary>
        /// 生成类似于p=>p.values.Contains("xxx");的lambda表达式
        /// parameterExpression标识p，propertyName表示values，propertyValue表示"xxx",methodName表示Contains
        /// 仅处理p的属性类型为string这种情况
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="parameterExpression"></param>
        /// <returns></returns>
        private static MethodCallExpression GetMethodExpression(string methodName, string propertyName, object propertyValue, ParameterExpression parameterExpression)
        {
            var propertyExpression = Expression.Property(parameterExpression, propertyName);
            MethodInfo method = typeof(string).GetMethod(methodName, new[] { typeof(string) });
            var someValue = Expression.Constant(propertyValue, typeof(string));
            return Expression.Call(propertyExpression, method, someValue);
        }
    }
}
