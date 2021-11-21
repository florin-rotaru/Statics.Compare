using AutoFixture;
using Models;
using Statics.Mapper;
using System;
using System.Linq;
using Xunit;
using static Statics.Compare.Members;

namespace Playground.Tests
{
    [Collection(nameof(Playground))]
    public class Defaults
    {
        private Fixture Fixture { get; }

        class TC1_0
        {
            public TC0_I0_Members[] Members { get; set; }
        }

        class TC2C1_0
        {
            public TC1_0 N1 { get; set; }
        }

        class TC_0
        {
            public TC2C1_0 N2 { get; set; }
        }

        public Defaults()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(Fixture);
        }

        private static bool IsNullableValueType(Type type) =>
            Nullable.GetUnderlyingType(type) != null;

        private static void CompareDefaults<L, R>()
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

    }
}
