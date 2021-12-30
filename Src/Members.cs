using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Statics.Compare
{
    public class Members
    {
        private const char DOT = '.';

        private static readonly MethodInfo CompareCollectionsMethodInfo =
            typeof(Members).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(m =>
                m.Name == nameof(CompareCollections) &&
                m.ReturnType == typeof(bool) &&
                m.IsGenericMethod);

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
                return Equals(leftValue.ToString(), rightValue.ToString());
            }
            else if (leftType == typeof(string))
            {
                return Equals(leftValue, rightValue.ToString());
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
                return Equals(rightValue.ToString(), leftValue.ToString());
            }
            else if (rightType == typeof(string))
            {
                return Equals(rightValue, leftValue.ToString());
            }
            else if (Reflection.TypeInfo.IsEnum(rightType))
            {
                return Equals(rightValue.ToString(), leftValue.ToString());
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

        private static bool IsCollection(Type type) =>
            !Reflection.TypeInfo.IsBuiltIn(type) &&
            (
                type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                Reflection.TypeInfo.ImplementsGenericTypeInterface(type, typeof(IEnumerable<>))
            );

        private static Type GetIEnumerableArgument(Type collectionType) =>
            collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ?
            collectionType.GenericTypeArguments[0] :
            Reflection.TypeInfo.GetGenericTypeInterface(collectionType, typeof(IEnumerable<>)).GenericTypeArguments[0];

        private static bool CompareCollections<L, R>(
            string leftMemberName,
            IEnumerable<L> left,
            string rightMemberName,
            IEnumerable<R> right,
            bool createMemberDiffs,
            out IEnumerable<MemberDiff> memberDiffs,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool areEqual = true,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false)
        {
            List<MemberDiff> diffs = new();
            memberDiffs = diffs;
            bool valid = true;

            if (left == null && right == null)
            {
                if (createMemberDiffs)
                    diffs.Add(new MemberDiff(
                        leftMemberName,
                        left,
                        rightMemberName,
                        right,
                        $"both {nameof(left)} and {nameof(right)} values are null"));

                memberDiffs = diffs;
                return areEqual == true;
            }

            if (left == null || right == null)
            {
                if (createMemberDiffs)
                    diffs.Add(new MemberDiff(
                        leftMemberName,
                        left,
                        rightMemberName,
                        right,
                        $"{nameof(left)} is {(left == null ? "null" : "not null")} and {nameof(right)} is {(right == null ? "null" : "not null")}"));

                memberDiffs = diffs;
                return areEqual != true;
            }

            L[] leftArray = left.ToArray();
            R[] rightArray = right.ToArray();

            if (leftArray.Length != rightArray.Length)
            {
                if (createMemberDiffs)
                    diffs.Add(new MemberDiff(
                        leftMemberName,
                        left,
                        rightMemberName,
                        right,
                        $"{nameof(left)} and {nameof(right)} collection size do not match"));

                memberDiffs = diffs;
                return areEqual != true;
            }

            for (int i = 0; i < leftArray.Length; i++)
            {
                if (!Compare(
                    $"{leftMemberName}[{i}]",
                    leftArray[i],
                    $"{rightMemberName}[{i}]",
                    rightArray[i],
                    createMemberDiffs,
                    out IEnumerable<MemberDiff> callMethodDiffs,
                    memberNameComparison,
                    true,
                    evaluateChildNodes,
                    ignoreDefaultLeftValues,
                    ignoreDefaultRightValues,
                    useConvert))
                {
                    valid = false;

                    if (createMemberDiffs)
                    {
                        diffs.AddRange(callMethodDiffs);
                        memberDiffs = diffs;
                    }
                    else
                    {
                        return valid;
                    }
                }
            }

            return valid;
        }

        private static bool InvokeCompareCollections(
            string leftMemberName,
            Type leftType,
            object leftValue,
            string rightMemberName,
            Type rightType,
            object rightValue,
            bool createMemberDiffs,
            out IEnumerable<MemberDiff> memberDiffs,
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

            object[] parameters = new object[]
            {
                leftMemberName,
                leftValue,
                rightMemberName,
                rightValue,
                createMemberDiffs,
                null,
                memberNameComparison,
                areEqual,
                evaluateChildNodes,
                ignoreDefaultLeftValues,
                ignoreDefaultRightValues,
                useConvert
            };

            bool valid = (bool)compareCollections
                .Invoke(null, parameters);

            memberDiffs = (IEnumerable<MemberDiff>)parameters[5];

            return valid;
        }

        private static string GetMemberMemberName(string memberName, Reflection.MemberInfo memberMember) =>
            $"{memberName}.{memberMember.Name}";

        private static bool Compare<L, R>(
            string leftMemberName,
            L left,
            string rightMemberName,
            R right,
            bool createMemberDiffs,
            out IEnumerable<MemberDiff> memberDiffs,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool areEqual = true,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false)
        {
            List<MemberDiff> diffs = new();
            IEnumerable<MemberDiff> callMethodDiffs;
            memberDiffs = diffs;
            bool valid = true;

            if (ignoreDefaultLeftValues && left == null)
            {
                if (createMemberDiffs)
                    diffs.Add(new MemberDiff(
                        leftMemberName,
                        left,
                        rightMemberName,
                        right,
                        $"{nameof(ignoreDefaultLeftValues)} is {ignoreDefaultLeftValues} and {nameof(left)} value is null"));

                memberDiffs = diffs;
                return areEqual == true;
            }

            if (ignoreDefaultRightValues && right == null)
            {
                if (createMemberDiffs)
                    diffs.Add(new MemberDiff(
                        leftMemberName,
                        left,
                        rightMemberName,
                        right,
                        $"{nameof(ignoreDefaultRightValues)} is {ignoreDefaultRightValues} and {nameof(right)} value is null"));

                memberDiffs = diffs;
                return areEqual == true;
            }

            if (left == null && right == null)
            {
                if (createMemberDiffs)
                    diffs.Add(new MemberDiff(
                        leftMemberName,
                        left,
                        rightMemberName,
                        right,
                        $"both {nameof(left)} and {nameof(right)} values are null"));

                memberDiffs = diffs;
                return areEqual == true;
            }

            if (left == null || right == null)
            {
                if (createMemberDiffs)
                    diffs.Add(new MemberDiff(
                        leftMemberName,
                        left,
                        rightMemberName,
                        right,
                        $"{nameof(left)} is {(left == null ? "null" : "not null")} and {nameof(right)} is {(right == null ? "null" : "not null")}"));

                memberDiffs = diffs;
                return areEqual != true;
            }

            Type leftType = typeof(L) != typeof(object) ? typeof(L) : (left != null ? left.GetType() : typeof(object));
            Type rightType = typeof(R) != typeof(object) ? typeof(R) : (right != null ? right.GetType() : typeof(object));

            if (Reflection.TypeInfo.IsBuiltIn(leftType) != Reflection.TypeInfo.IsBuiltIn(rightType))
                throw new NotSupportedException();

            if (Reflection.TypeInfo.IsBuiltIn(leftType) && Reflection.TypeInfo.IsBuiltIn(rightType))
            {
                valid = BuiltInTypesEquals(
                    leftType,
                    left,
                    rightType,
                    right,
                    ignoreDefaultLeftValues,
                    ignoreDefaultRightValues,
                    useConvert) == areEqual;

                if (!valid && createMemberDiffs)
                    diffs.Add(new MemberDiff(
                        leftMemberName,
                        left,
                        rightMemberName,
                        right,
                        $"value: {left} does not equal{(useConvert ? " nor can be converted" : string.Empty)} to: {right}"));

                memberDiffs = diffs;

                return valid;
            }

            if (IsCollection(leftType) &&
                IsCollection(rightType))
            {
                valid = InvokeCompareCollections(
                    leftMemberName,
                    leftType,
                    left,
                    rightMemberName,
                    rightType,
                    right,
                    createMemberDiffs,
                    out callMethodDiffs,
                    memberNameComparison,
                    areEqual,
                    evaluateChildNodes,
                    ignoreDefaultLeftValues,
                    ignoreDefaultRightValues,
                    useConvert);

                if (!valid && createMemberDiffs)
                    diffs.AddRange(callMethodDiffs);

                return valid;
            }

            IEnumerable<Reflection.MemberInfo> leftMembers = Reflection.TypeInfo.GetGettableMembers(leftType, true);
            IEnumerable<Reflection.MemberInfo> rightMembers = Reflection.TypeInfo.GetGettableMembers(rightType, true);

            string leftMemberMemberName;
            string rightMemberMemberName;

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
                        leftMemberMemberName = GetMemberMemberName(leftMemberName, leftMember);
                        rightMemberMemberName = GetMemberMemberName(rightMemberName, rightMember);

                        try { rightMemberValue = rightMember.GetValue(right); }
                        catch { continue; }

                        if (leftMember.IsBuiltIn && rightMember.IsBuiltIn)
                        {
                            if (ignoreDefaultRightValues && ObjectEquals(rightMemberValue, rightMember.DefaultValue, false)) break;

                            if (leftMember.IsEnum)
                            {
                                if (EnumEqualsTo(leftMember, leftMemberValue, rightMember, rightMemberValue) != areEqual)
                                {
                                    if (createMemberDiffs)
                                    {
                                        diffs.Add(new MemberDiff(
                                            leftMemberMemberName,
                                            leftMemberValue,
                                            rightMemberMemberName,
                                            rightMemberValue,
                                            $"value {leftMemberValue} does not equal nor can be converted to {rightMemberValue}"));

                                        valid = false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else if (rightMember.IsEnum)
                            {
                                if (EqualsToEnum(leftMember, leftMemberValue, rightMember, rightMemberValue) != areEqual)
                                {
                                    if (createMemberDiffs)
                                    {
                                        diffs.Add(new MemberDiff(
                                            leftMemberMemberName,
                                            leftMemberValue,
                                            rightMemberMemberName,
                                            rightMemberValue,
                                            $"value {leftMemberValue} does not equal nor can be converted to {rightMemberValue}"));

                                        valid = false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (ObjectEquals(leftMemberValue, rightMemberValue, useConvert) != areEqual)
                                {
                                    if (createMemberDiffs)
                                    {
                                        diffs.Add(new MemberDiff(
                                            leftMemberMemberName,
                                            leftMemberValue,
                                            rightMemberMemberName,
                                            rightMemberValue,
                                            $"value {leftMemberValue} does not equal{(useConvert ? " nor can be converted" : string.Empty)} to {rightMemberValue}"));

                                        valid = false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }

                            break;
                        }

                        if (IsCollection(leftMember.Type) &&
                            IsCollection(rightMember.Type) &&
                            !InvokeCompareCollections(
                                leftMemberMemberName,
                                leftMember.Type,
                                leftMemberValue,
                                rightMemberMemberName,
                                rightMember.Type,
                                rightMemberValue,
                                createMemberDiffs,
                                out callMethodDiffs,
                                memberNameComparison,
                                areEqual,
                                evaluateChildNodes,
                                ignoreDefaultLeftValues,
                                ignoreDefaultRightValues,
                                useConvert))
                        {
                            if (createMemberDiffs)
                            {
                                diffs.AddRange(callMethodDiffs);
                                valid = false;
                            }
                            else
                            {
                                return false;
                            }
                        }

                        if (rightMember.IsEnumerable && !rightMember.IsBuiltIn) break;

                        if (!rightMember.IsBuiltIn && !rightMember.IsEnum)
                        {
                            if (!evaluateChildNodes) break;

                            if (!Compare(
                                leftMemberMemberName,
                                leftMemberValue,
                                rightMemberMemberName,
                                rightMemberValue,
                                createMemberDiffs,
                                out callMethodDiffs,
                                memberNameComparison,
                                areEqual,
                                evaluateChildNodes,
                                ignoreDefaultLeftValues,
                                ignoreDefaultRightValues,
                                useConvert))
                            {
                                if (createMemberDiffs)
                                {
                                    diffs.AddRange(callMethodDiffs);
                                    valid = false;
                                }
                                else 
                                {
                                    return false;
                                }
                            }

                            break;
                        }

                        break;
                    }
                }
            }

            return valid;
        }

        public static bool CompareEquals<L, R>(
            L left,
            R right,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false) =>
            Compare(
                nameof(L),
                left,
                nameof(R),
                right,
                false,
                out _,
                memberNameComparison,
                true,
                evaluateChildNodes,
                ignoreDefaultLeftValues,
                ignoreDefaultRightValues,
                useConvert);

        public static bool CompareEquals<L, R>(
            L left,
            R right,
            out IEnumerable<MemberDiff> memberDiffs,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false) =>
            Compare(
                nameof(L),
                left,
                nameof(R),
                right,
                true,
                out memberDiffs,
                memberNameComparison,
                true,
                evaluateChildNodes,
                ignoreDefaultLeftValues,
                ignoreDefaultRightValues,
                useConvert);

        public static bool CompareNoneEquals<L, R>(
            L left,
            R right,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false) =>
            Compare(
                nameof(L),
                left,
                nameof(R),
                right,
                true,
                out _,
                memberNameComparison,
                false,
                evaluateChildNodes,
                ignoreDefaultLeftValues,
                ignoreDefaultRightValues,
                useConvert);

        public static bool CompareNoneEquals<L, R>(
            L left,
            R right,
            out IEnumerable<MemberDiff> memberDiffs,
            StringComparison memberNameComparison = StringComparison.OrdinalIgnoreCase,
            bool evaluateChildNodes = false,
            bool ignoreDefaultLeftValues = false,
            bool ignoreDefaultRightValues = false,
            bool useConvert = false) =>
            Compare(
                nameof(L),
                left,
                nameof(R),
                right,
                true,
                out memberDiffs,
                memberNameComparison,
                false,
                evaluateChildNodes,
                ignoreDefaultLeftValues,
                ignoreDefaultRightValues,
                useConvert);
    }
}
