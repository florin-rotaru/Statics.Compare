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

        private static readonly MethodInfo ObjectToString = typeof(object).GetMethod(nameof(object.ToString), new Type[] { });

        private static object InvokeObjectToString(object value) =>
            ObjectToString.Invoke(value, null);

        private static bool ObjectEquals(
            object left,
            object right,
            bool useConvert)
        {
            if (!useConvert)
                return Equals(left, right);

            Type leftType = left.GetType();

            if (leftType == right.GetType())
                return Equals(left, right);

            object adaptRight = Convert.ChangeType(right, leftType);
            return Equals(left, adaptRight);
        }

        private static bool EqualsToEnum(
            Reflection.MemberInfo leftMember,
            object leftMemberValue,
            Reflection.MemberInfo rightMember,
            object rightMemberValue) =>
            EqualsToEnum(leftMember.Type, leftMember.IsNumeric, leftMemberValue, rightMember.Type, rightMemberValue);

        private static bool EqualsToEnum(
            Type leftType,
            bool leftIsNumeric,
            object leftValue,
            Type rightType,
            object rightValue)
        {
            if (leftValue == rightValue)
                return true;

            if (leftValue == null || rightValue == null)
                return false;

            if (leftIsNumeric)
            {
                object adaptRightValue = Convert.ChangeType(rightValue, Enum.GetUnderlyingType(rightType));

                if (leftType == Enum.GetUnderlyingType(rightType))
                    return Equals(leftValue, adaptRightValue);

                return Equals(Convert.ChangeType(leftValue, Enum.GetUnderlyingType(rightType)), adaptRightValue);
            }
            else if (leftType == typeof(char))
            {
                return Equals(InvokeObjectToString(leftValue), InvokeObjectToString(rightValue));
            }
            else if (leftType == typeof(string))
            {
                return Equals(leftValue, InvokeObjectToString(rightValue));
            }

            return false;
        }

        private static bool EnumEqualsTo(
            Reflection.MemberInfo leftMember,
            object leftMemberValue,
            Reflection.MemberInfo rightMember,
            object rightMemberValue) =>
            EnumEqualsTo(leftMember.Type, leftMemberValue, rightMember.Type, rightMember.IsNumeric, rightMemberValue);

        private static bool EnumEqualsTo(
            Type leftType,
            object leftValue,
            Type rightType,
            bool rightIsNumeric,
            object rightValue)
        {
            if (leftValue == rightValue)
                return true;

            if (leftValue == null || rightValue == null)
                return false;

            if (rightIsNumeric)
            {
                object adaptLeftValue = Convert.ChangeType(leftValue, Enum.GetUnderlyingType(leftType));

                if (rightType == Enum.GetUnderlyingType(leftType))
                    return Equals(adaptLeftValue, rightValue);

                return Equals(Convert.ChangeType(rightValue, Enum.GetUnderlyingType(leftType)), adaptLeftValue);
            }
            else if (rightType == typeof(char))
            {
                return Equals(InvokeObjectToString(rightValue), InvokeObjectToString(leftValue));
            }
            else if (rightType == typeof(string))
            {
                return Equals(rightValue, InvokeObjectToString(leftValue));
            }
            else if (Reflection.TypeInfo.IsEnum(rightType))
            {
                return Equals(InvokeObjectToString(rightValue), InvokeObjectToString(leftValue));
            }

            return false;
        }

        private static bool BuiltInTypesEquals(
            Type leftType,
            object leftValue,
            Type rightType,
            object rightValue,
            bool ignoreDefaultLeftValues,
            bool ignoreDefaultRightValues,
            bool useConvert)
        {
            if (ignoreDefaultLeftValues && leftValue.Equals(Reflection.TypeInfo.GetDefaultValue(leftType)))
                return true;

            if (ignoreDefaultRightValues && rightValue.Equals(Reflection.TypeInfo.GetDefaultValue(rightType)))
                return true;

            if (!useConvert)
                return Equals(leftValue, rightValue);

            if (Reflection.TypeInfo.IsEnum(leftType))
                return EnumEqualsTo(leftType, leftValue, rightType, Reflection.TypeInfo.IsNumeric(rightType), rightValue);

            if (Reflection.TypeInfo.IsEnum(rightType))
                return EqualsToEnum(leftType, Reflection.TypeInfo.IsNumeric(leftType), leftValue, rightType, rightValue);

            return Equals(leftValue, Convert.ChangeType(rightValue, leftType));
        }

        public static bool IsCollection(Type type) =>
            !Reflection.TypeInfo.IsBuiltIn(type) &&
            (
                type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                Reflection.TypeInfo.ImplementsGenericTypeInterface(type, typeof(IEnumerable<>))
            );

        public static Type GetIEnumerableArgument(Type collectionType) =>
            collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ?
            collectionType.GenericTypeArguments[0] :
            Reflection.TypeInfo.GetGenericTypeInterface(collectionType, typeof(IEnumerable<>)).GenericTypeArguments[0];

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
                    GetIEnumerableArgument(leftType),
                    GetIEnumerableArgument(rightType)
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
            if (ignoreDefaultLeftValues && left == null)
                return areEqual == true;

            if (ignoreDefaultRightValues && right == null)
                return areEqual == true;

            if (left == null && right == null)
                return areEqual == true;

            if (left == null || right == null)
                return areEqual != true;

            Type leftType = typeof(L) != typeof(object) ? typeof(L) : (left != null ? left.GetType() : typeof(object));
            Type rightType = typeof(R) != typeof(object) ? typeof(R) : (right != null ? right.GetType() : typeof(object));

            if (Reflection.TypeInfo.IsBuiltIn(leftType) != Reflection.TypeInfo.IsBuiltIn(rightType))
                throw new NotSupportedException();

            if (Reflection.TypeInfo.IsBuiltIn(leftType) && Reflection.TypeInfo.IsBuiltIn(rightType))
                return BuiltInTypesEquals(
                    leftType,
                    left,
                    rightType,
                    right,
                    ignoreDefaultLeftValues,
                    ignoreDefaultRightValues,
                    useConvert) == areEqual;

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

            IEnumerable<Reflection.MemberInfo> leftMembers = Reflection.TypeInfo.GetGettableMembers(leftType, true);
            IEnumerable<Reflection.MemberInfo> rightMembers = Reflection.TypeInfo.GetGettableMembers(rightType, true);

            object leftMemberValue = null;
            object rightMemberValue = null;

            foreach (Reflection.MemberInfo leftMember in leftMembers)
            {
                try { leftMemberValue = leftMember.GetValue(left); }
                catch { continue; }

                if (ignoreDefaultLeftValues && ObjectEquals(leftMemberValue, leftMember.DefaultValue, false)) continue;

                foreach (Reflection.MemberInfo rightMember in rightMembers)
                {
                    if (rightMember.Name.Equals(leftMember.Name, memberNameComparison))
                    {
                        try { rightMemberValue = rightMember.GetValue(right); }
                        catch { continue; }

                        if (leftMember.IsBuiltIn && rightMember.IsBuiltIn)
                        {
                            if (ignoreDefaultRightValues && ObjectEquals(rightMemberValue, rightMember.DefaultValue, false)) break;

                            if (leftMember.IsEnum)
                            { if (EnumEqualsTo(leftMember, leftMemberValue, rightMember, rightMemberValue) != areEqual) return false; }
                            else if (rightMember.IsEnum)
                            { if (EqualsToEnum(leftMember, leftMemberValue, rightMember, rightMemberValue) != areEqual) return false; }
                            else
                            { if (ObjectEquals(leftMemberValue, rightMemberValue, useConvert) != areEqual) return false; }

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
