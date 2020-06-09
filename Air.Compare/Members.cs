using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Air.Compare
{
    public class Members
    {
        private const char DOT = '.';

        private static readonly MethodInfo CompareCollectionsMethodInfo =
            typeof(Members).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(m =>
                m.Name == nameof(CompareCollections) &&
                m.ReturnType == typeof(bool) &&
                m.IsGenericMethod &&
                m.GetParameters().FirstOrDefault(p => p.Position == 0 && p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) != null &&
                m.GetParameters().FirstOrDefault(p => p.Position == 1 && p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) != null);

        static bool ObjectEquals(
            object left,
            object right,
            bool useConvert)
        {
            if (!useConvert)
                return object.Equals(left, right);

            Type leftType = left.GetType();

            if (leftType == right.GetType())
                return object.Equals(left, right);

            object adaptRight = Convert.ChangeType(right, leftType);
            return object.Equals(left, adaptRight);
        }

        private static bool IsCollection(Type type)
        {
            if (type.IsArray)
                return true;

            if (!type.IsGenericType)
                return false;

            for (int i = 0; i < type.GetInterfaces().Length; i++)
                if (type.GetInterfaces()[i].IsGenericType &&
                    type.GetInterfaces()[i].GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return true;

            return false;
        }

        private static Type GetCollectionGenericType(Type collection)
        {
            if (collection.IsArray)
                return collection.GetElementType();

            if (collection.IsInterface &&
                collection.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return collection.GenericTypeArguments[0];

            return
                collection
                .GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .GenericTypeArguments[0];
        }

        private static bool CompareCollections<L, R>(
            IEnumerable<L> left,
            IEnumerable<R> right,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool areEqual = true,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false)
        {
            if (left == null && right == null)
                return areEqual == true;

            if (left == null || right == null)
                return areEqual != true;

            L[] leftArray = left.ToArray();
            R[] rightArray = right.ToArray();

            if (leftArray.Length != rightArray.Length)
                return areEqual != true;

            for (int i = 0; i < leftArray.Length; i++)
            {
                if (!Compare(
                    leftArray[i],
                    rightArray[i],
                    memberNameComparison,
                    true,
                    evaluateChildNodes,
                    ignoreDefaultLeftValues,
                    ignoreDefaultRightValues,
                    useConvert))
                    return false;
            }

            return true;
        }

        private static bool InvokeCompareCollections(
            Type leftType,
            object leftValue,
            Type rightType,
            object rightValue,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool areEqual = true,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false)
        {
            MethodInfo compareCollections = CompareCollectionsMethodInfo
                .MakeGenericMethod(new Type[]
                {
                    GetCollectionGenericType(leftType),
                    GetCollectionGenericType(rightType)
                });

            return (bool)compareCollections
                .Invoke(null, new object[]
                {
                    leftValue,
                    rightValue,
                    memberNameComparison,
                    areEqual,
                    evaluateChildNodes,
                    ignoreDefaultLeftValues,
                    ignoreDefaultRightValues,
                    useConvert
                });
        }

        private static bool Compare<L, R>(
            L left,
            R right,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool areEqual = true,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false)
        {
            if (left == default && ignoreDefaultLeftValues)
                return areEqual == true;

            if (right == default && ignoreDefaultRightValues)
                return areEqual == true;

            if (left == default && right == default)
                return areEqual == true;

            if (left == default || right == default)
                return areEqual != true;

            Type leftType = typeof(L) != typeof(object) ? typeof(L) : left.GetType();
            Type rightType = typeof(R) != typeof(object) ? typeof(R) : right.GetType();

            if (Reflection.TypeInfo.IsBuiltIn(leftType) && Reflection.TypeInfo.IsBuiltIn(rightType))
                return ObjectEquals(left, right, useConvert) == areEqual;

            if (Reflection.TypeInfo.IsBuiltIn(leftType) != Reflection.TypeInfo.IsBuiltIn(rightType))
                throw new NotSupportedException();

            if (IsCollection(leftType) &&
                IsCollection(rightType))
                return InvokeCompareCollections(
                    leftType,
                    left,
                    rightType,
                    right,
                    memberNameComparison,
                    areEqual,
                    evaluateChildNodes,
                    ignoreDefaultLeftValues,
                    ignoreDefaultRightValues,
                    useConvert);

            List<Reflection.MemberInfo> leftMembers = Reflection.TypeInfo.GetGettableMembers(leftType, true);
            List<Reflection.MemberInfo> rightMembers = Reflection.TypeInfo.GetGettableMembers(rightType, true);

            object leftMemberValue = null;
            object rightMemberValue = null;

            foreach (Reflection.MemberInfo leftMember in leftMembers)
            {
                try { leftMemberValue = leftMember.GetValue(left); }
                catch { continue; }

                if (ignoreDefaultLeftValues && ObjectEquals(leftMemberValue, leftMember.DefaultValue, useConvert)) continue;

                foreach (Reflection.MemberInfo rightMember in rightMembers)
                {
                    if (rightMember.Name.Equals(leftMember.Name, memberNameComparison))
                    {
                        try { rightMemberValue = rightMember.GetValue(right); }
                        catch { continue; }

                        if (leftMember.IsBuiltIn && rightMember.IsBuiltIn)
                        {
                            if (ignoreDefaultRightValues && ObjectEquals(rightMemberValue, rightMember.DefaultValue, useConvert)) break;
                            if (ObjectEquals(leftMemberValue, rightMemberValue, useConvert) != areEqual) return false;

                            break;
                        }

                        if (IsCollection(leftMember.Type) &&
                            IsCollection(rightMember.Type) &&
                            !InvokeCompareCollections(
                                leftMember.Type,
                                leftMemberValue,
                                rightMember.Type,
                                rightMemberValue,
                                memberNameComparison,
                                areEqual,
                                evaluateChildNodes,
                                ignoreDefaultLeftValues,
                                ignoreDefaultRightValues,
                                useConvert))
                            return false;

                        if (rightMember.IsEnumerable && !rightMember.IsBuiltIn) break;

                        if (!rightMember.IsBuiltIn && !rightMember.IsEnum)
                        {
                            if (!evaluateChildNodes) break;

                            if (!Compare(
                                leftMemberValue,
                                rightMemberValue,
                                memberNameComparison,
                                areEqual,
                                evaluateChildNodes,
                                ignoreDefaultLeftValues,
                                ignoreDefaultRightValues,
                                useConvert))
                                return false;

                            break;
                        }

                        break;
                    }
                }
            }

            return true;
        }

        public static bool CompareEquals<L, R>(
            L left,
            R right,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false)
        {
            return Compare(
                left,
                right,
                memberNameComparison,
                true,
                evaluateChildNodes,
                ignoreDefaultLeftValues,
                ignoreDefaultRightValues,
                useConvert);
        }

        public static bool CompareNoneEquals<L, R>(
            L left,
            R right,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false)
        {
            return Compare(
                left,
                right,
                memberNameComparison,
                false,
                evaluateChildNodes,
                ignoreDefaultLeftValues,
                ignoreDefaultRightValues,
                useConvert);
        }
    }
}
