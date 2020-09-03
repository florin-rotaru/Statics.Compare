using Air.Mapper;
using AutoFixture;
using Models;
using System;
using System.Linq;
using Xunit;
using static Air.Compare.Members;

namespace Playground
{
    [Collection(nameof(Playground))]
    public class Types
    {
        private Fixture Fixture { get; }

        public Types()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(Fixture);
        }

        private bool IsNullableValueType(Type type) =>
            Nullable.GetUnderlyingType(type) != null;

        private void CompareDefaults<L, R>()
            where L : new()
            where R : new()
        {
            Type leftType = typeof(L);
            Type rightType = typeof(R);

            L left = default;
            R right = default;

            if (IsNullableValueType(leftType))
            {
                if (IsNullableValueType(rightType))
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
                else if (rightType.IsValueType)
                    Assert.False(CompareEquals(left, right));
                else
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
            }
            else if (typeof(L).IsValueType)
            {
                if (IsNullableValueType(rightType))
                    Assert.False(CompareEquals(left, right));
                else if (rightType.IsValueType)
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
                else
                    Assert.False(CompareEquals(left, right));
            }
            else
            {
                if (IsNullableValueType(rightType))
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
                else if (rightType.IsValueType)
                    Assert.False(CompareEquals(left, right));
                else
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
            }

            // =======
            left = new L();

            if (IsNullableValueType(leftType))
            {
                if (IsNullableValueType(rightType))
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
                else if (rightType.IsValueType)
                    Assert.False(CompareEquals(left, right));
                else
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
            }
            else if (typeof(L).IsValueType)
            {
                if (IsNullableValueType(rightType))
                    Assert.False(CompareEquals(left, right));
                else if (rightType.IsValueType)
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
                else
                    Assert.False(CompareEquals(left, right));
            }
            else
            {
                if (IsNullableValueType(rightType))
                    Assert.False(CompareEquals(left, right));
                else if (rightType.IsValueType)
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
                else
                    Assert.False(CompareEquals(left, right));
            }

            // =======
            left = default;
            right = new R();

            if (IsNullableValueType(leftType))
            {
                if (IsNullableValueType(rightType))
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
                else if (rightType.IsValueType)
                    Assert.False(CompareEquals(left, right));
                else
                    Assert.False(CompareEquals(left, right));
            }
            else if (typeof(L).IsValueType)
            {
                if (IsNullableValueType(rightType))
                    Assert.False(CompareEquals(left, right));
                else if (rightType.IsValueType)
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
                else
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
            }
            else
            {
                if (IsNullableValueType(rightType))
                    Assert.True(CompareEquals(left, right, ignoreDefaultRightValues: true));
                else if (rightType.IsValueType)
                    Assert.False(CompareEquals(left, right));
                else
                    Assert.False(CompareEquals(left, right));
            }
        }

        private void CompareEnumerable<L, R>()
        {
            var entries = Fixture.Create<TC0_I0_Members[]>();
            var leftEntries = Mapper<TC0_I0_Members[], L[]>.Map(entries);
            var rightEntries = Mapper<L[], R[]>.Map(leftEntries);
            Assert.True(CompareEquals(leftEntries, rightEntries));

            leftEntries[0] = Mapper<TC0_I0_Members, L>.Map(Fixture.Create<TC0_I0_Members>());
            Assert.False(CompareEquals(leftEntries, rightEntries));
        }

        [Fact]
        public void FromToDefaults()
        {
            CompareDefaults<TC0_I0_Members, TC0_I0_Members>();
            CompareDefaults<TC0_I0_Members, TC0_I1_Nullable_Members>();
            CompareDefaults<TC0_I0_Members, TC0_I4_Static_Members>();
            CompareDefaults<TC0_I0_Members, TC0_I5_StaticNullable_Members>();
            CompareDefaults<TC0_I0_Members, TS0_I0_Members>();
            CompareDefaults<TC0_I0_Members, TS0_I1_Nullable_Members>();
            CompareDefaults<TC0_I0_Members, TS0_I3_Static_Members>();
            CompareDefaults<TC0_I0_Members, TS0_I4_StaticNullable_Members>();

            CompareDefaults<TC0_I1_Nullable_Members, TC0_I0_Members>();
            CompareDefaults<TC0_I1_Nullable_Members, TC0_I1_Nullable_Members>();
            CompareDefaults<TC0_I1_Nullable_Members, TC0_I4_Static_Members>();
            CompareDefaults<TC0_I1_Nullable_Members, TC0_I5_StaticNullable_Members>();
            CompareDefaults<TC0_I1_Nullable_Members, TS0_I0_Members>();
            CompareDefaults<TC0_I1_Nullable_Members, TS0_I1_Nullable_Members>();
            CompareDefaults<TC0_I1_Nullable_Members, TS0_I3_Static_Members>();
            CompareDefaults<TC0_I1_Nullable_Members, TS0_I4_StaticNullable_Members>();

            CompareDefaults<TC0_I4_Static_Members, TC0_I0_Members>();
            CompareDefaults<TC0_I4_Static_Members, TC0_I1_Nullable_Members>();
            CompareDefaults<TC0_I4_Static_Members, TC0_I4_Static_Members>();
            CompareDefaults<TC0_I4_Static_Members, TC0_I5_StaticNullable_Members>();
            CompareDefaults<TC0_I4_Static_Members, TS0_I0_Members>();
            CompareDefaults<TC0_I4_Static_Members, TS0_I1_Nullable_Members>();
            CompareDefaults<TC0_I4_Static_Members, TS0_I3_Static_Members>();
            CompareDefaults<TC0_I4_Static_Members, TS0_I4_StaticNullable_Members>();

            CompareDefaults<TC0_I5_StaticNullable_Members, TC0_I0_Members>();
            CompareDefaults<TC0_I5_StaticNullable_Members, TC0_I1_Nullable_Members>();
            CompareDefaults<TC0_I5_StaticNullable_Members, TC0_I4_Static_Members>();
            CompareDefaults<TC0_I5_StaticNullable_Members, TC0_I5_StaticNullable_Members>();
            CompareDefaults<TC0_I5_StaticNullable_Members, TS0_I0_Members>();
            CompareDefaults<TC0_I5_StaticNullable_Members, TS0_I1_Nullable_Members>();
            CompareDefaults<TC0_I5_StaticNullable_Members, TS0_I3_Static_Members>();
            CompareDefaults<TC0_I5_StaticNullable_Members, TS0_I4_StaticNullable_Members>();

            CompareDefaults<TS0_I0_Members, TC0_I0_Members>();
            CompareDefaults<TS0_I0_Members, TC0_I1_Nullable_Members>();
            CompareDefaults<TS0_I0_Members, TC0_I4_Static_Members>();
            CompareDefaults<TS0_I0_Members, TC0_I5_StaticNullable_Members>();
            CompareDefaults<TS0_I0_Members, TS0_I0_Members>();
            CompareDefaults<TS0_I0_Members, TS0_I1_Nullable_Members>();
            CompareDefaults<TS0_I0_Members, TS0_I3_Static_Members>();
            CompareDefaults<TS0_I0_Members, TS0_I4_StaticNullable_Members>();

            CompareDefaults<TS0_I1_Nullable_Members, TC0_I0_Members>();
            CompareDefaults<TS0_I1_Nullable_Members, TC0_I1_Nullable_Members>();
            CompareDefaults<TS0_I1_Nullable_Members, TC0_I4_Static_Members>();
            CompareDefaults<TS0_I1_Nullable_Members, TC0_I5_StaticNullable_Members>();
            CompareDefaults<TS0_I1_Nullable_Members, TS0_I0_Members>();
            CompareDefaults<TS0_I1_Nullable_Members, TS0_I1_Nullable_Members>();
            CompareDefaults<TS0_I1_Nullable_Members, TS0_I3_Static_Members>();
            CompareDefaults<TS0_I1_Nullable_Members, TS0_I4_StaticNullable_Members>();

            CompareDefaults<TS0_I3_Static_Members, TC0_I0_Members>();
            CompareDefaults<TS0_I3_Static_Members, TC0_I1_Nullable_Members>();
            CompareDefaults<TS0_I3_Static_Members, TC0_I4_Static_Members>();
            CompareDefaults<TS0_I3_Static_Members, TC0_I5_StaticNullable_Members>();
            CompareDefaults<TS0_I3_Static_Members, TS0_I0_Members>();
            CompareDefaults<TS0_I3_Static_Members, TS0_I1_Nullable_Members>();
            CompareDefaults<TS0_I3_Static_Members, TS0_I3_Static_Members>();
            CompareDefaults<TS0_I3_Static_Members, TS0_I4_StaticNullable_Members>();

            CompareDefaults<TS0_I4_StaticNullable_Members, TC0_I0_Members>();
            CompareDefaults<TS0_I4_StaticNullable_Members, TC0_I1_Nullable_Members>();
            CompareDefaults<TS0_I4_StaticNullable_Members, TC0_I4_Static_Members>();
            CompareDefaults<TS0_I4_StaticNullable_Members, TC0_I5_StaticNullable_Members>();
            CompareDefaults<TS0_I4_StaticNullable_Members, TS0_I0_Members>();
            CompareDefaults<TS0_I4_StaticNullable_Members, TS0_I1_Nullable_Members>();
            CompareDefaults<TS0_I4_StaticNullable_Members, TS0_I3_Static_Members>();
            CompareDefaults<TS0_I4_StaticNullable_Members, TS0_I4_StaticNullable_Members>();
        }

        [Fact]
        public void FromToEnum()
        {
            TS0_I1_Nullable_Members left = new TS0_I1_Nullable_Members();
            TS0_I1_Nullable_Members right = new TS0_I1_Nullable_Members();
            Assert.True(CompareEquals(left.EnumMember, right.UndefinedEnumMember, useConvert: true));

            left.EnumMember = TABCEnum.B;
            Assert.False(CompareEquals(left.EnumMember, right.UndefinedEnumMember, useConvert: true));

            left.EnumMember = null;
            right.UndefinedEnumMember = TUndefinedABCEnum.B;
            Assert.False(CompareEquals(left.EnumMember, right.UndefinedEnumMember, useConvert: true));

            Assert.True(CompareEquals((int)TABCEnum.B, TABCEnum.B, useConvert: true));
            Assert.True(CompareEquals(TABCEnum.B, (int)TABCEnum.B, useConvert: true));
            Assert.True(CompareEquals("B", TABCEnum.B, useConvert: true));
            Assert.True(CompareEquals(TABCEnum.B, "B", useConvert: true));
            Assert.True(CompareEquals('B', TABCEnum.B, useConvert: true));
            Assert.True(CompareEquals(TABCEnum.B, 'B', useConvert: true));
            Assert.True(CompareEquals(TUndefinedABCEnum.B, TABCEnum.B, useConvert: true));
            Assert.True(CompareEquals(TABCEnum.B, TUndefinedABCEnum.B, useConvert: true));
        }

        [Fact]
        public void FromToEnumerable()
        {
            CompareEnumerable<TC0_I0_Members, TC0_I0_Members>();
            CompareEnumerable<TC0_I0_Members, TC0_I1_Nullable_Members>();
            CompareEnumerable<TC0_I0_Members, TS0_I0_Members>();
            CompareEnumerable<TC0_I0_Members, TS0_I1_Nullable_Members>();

            CompareEnumerable<TC0_I1_Nullable_Members, TC0_I0_Members>();
            CompareEnumerable<TC0_I1_Nullable_Members, TC0_I1_Nullable_Members>();
            CompareEnumerable<TC0_I1_Nullable_Members, TS0_I0_Members>();
            CompareEnumerable<TC0_I1_Nullable_Members, TS0_I1_Nullable_Members>();

            CompareEnumerable<TC0_I4_Static_Members, TC0_I0_Members>();
            CompareEnumerable<TC0_I4_Static_Members, TC0_I1_Nullable_Members>();
            CompareEnumerable<TC0_I4_Static_Members, TS0_I0_Members>();
            CompareEnumerable<TC0_I4_Static_Members, TS0_I1_Nullable_Members>();

            CompareEnumerable<TC0_I5_StaticNullable_Members, TC0_I0_Members>();
            CompareEnumerable<TC0_I5_StaticNullable_Members, TC0_I1_Nullable_Members>();
            CompareEnumerable<TC0_I5_StaticNullable_Members, TS0_I0_Members>();
            CompareEnumerable<TC0_I5_StaticNullable_Members, TS0_I1_Nullable_Members>();

            CompareEnumerable<TS0_I0_Members, TC0_I0_Members>();
            CompareEnumerable<TS0_I0_Members, TC0_I1_Nullable_Members>();
            CompareEnumerable<TS0_I0_Members, TS0_I0_Members>();
            CompareEnumerable<TS0_I0_Members, TS0_I1_Nullable_Members>();

            CompareEnumerable<TS0_I1_Nullable_Members, TC0_I0_Members>();
            CompareEnumerable<TS0_I1_Nullable_Members, TC0_I1_Nullable_Members>();
            CompareEnumerable<TS0_I1_Nullable_Members, TS0_I0_Members>();
            CompareEnumerable<TS0_I1_Nullable_Members, TS0_I1_Nullable_Members>();

            CompareEnumerable<TS0_I3_Static_Members, TC0_I0_Members>();
            CompareEnumerable<TS0_I3_Static_Members, TC0_I1_Nullable_Members>();
            CompareEnumerable<TS0_I3_Static_Members, TS0_I0_Members>();
            CompareEnumerable<TS0_I3_Static_Members, TS0_I1_Nullable_Members>();

            CompareEnumerable<TS0_I4_StaticNullable_Members, TC0_I0_Members>();
            CompareEnumerable<TS0_I4_StaticNullable_Members, TC0_I1_Nullable_Members>();
            CompareEnumerable<TS0_I4_StaticNullable_Members, TS0_I0_Members>();
            CompareEnumerable<TS0_I4_StaticNullable_Members, TS0_I1_Nullable_Members>();
        }
    }
}
