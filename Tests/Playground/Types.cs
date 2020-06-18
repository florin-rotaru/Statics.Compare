using Air.Mapper;
using AutoFixture;
using Models;
using System;
using System.Linq;
using Xunit;
using static Air.Compare.Members;

namespace Playground
{
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

        private void CompareDefaults<S, D>()
            where S : new()
            where D : new()
        {
            Type sourceType = typeof(S);
            Type destinationType = typeof(D);

            S source = default;
            D destination = default;

            if (IsNullableValueType(sourceType))
            {
                if (IsNullableValueType(destinationType))
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
                else if (destinationType.IsValueType)
                    Assert.False(CompareEquals(source, destination));
                else
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
            }
            else if (typeof(S).IsValueType)
            {
                if (IsNullableValueType(destinationType))
                    Assert.False(CompareEquals(source, destination));
                else if (destinationType.IsValueType)
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
                else
                    Assert.False(CompareEquals(source, destination));
            }
            else
            {
                if (IsNullableValueType(destinationType))
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
                else if (destinationType.IsValueType)
                    Assert.False(CompareEquals(source, destination));
                else
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
            }

            // =======
            source = new S();

            if (IsNullableValueType(sourceType))
            {
                if (IsNullableValueType(destinationType))
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
                else if (destinationType.IsValueType)
                    Assert.False(CompareEquals(source, destination));
                else
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
            }
            else if (typeof(S).IsValueType)
            {
                if (IsNullableValueType(destinationType))
                    Assert.False(CompareEquals(source, destination));
                else if (destinationType.IsValueType)
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
                else
                    Assert.False(CompareEquals(source, destination));
            }
            else
            {
                if (IsNullableValueType(destinationType))
                    Assert.False(CompareEquals(source, destination));
                else if (destinationType.IsValueType)
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
                else
                    Assert.False(CompareEquals(source, destination));
            }

            // =======
            source = default;
            destination = new D();

            if (IsNullableValueType(sourceType))
            {
                if (IsNullableValueType(destinationType))
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
                else if (destinationType.IsValueType)
                    Assert.False(CompareEquals(source, destination));
                else
                    Assert.False(CompareEquals(source, destination));
            }
            else if (typeof(S).IsValueType)
            {
                if (IsNullableValueType(destinationType))
                    Assert.False(CompareEquals(source, destination));
                else if (destinationType.IsValueType)
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
                else
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
            }
            else
            {
                if (IsNullableValueType(destinationType))
                    Assert.True(CompareEquals(source, destination, ignoreDefaultRightValues: true));
                else if (destinationType.IsValueType)
                    Assert.False(CompareEquals(source, destination));
                else
                    Assert.False(CompareEquals(source, destination));
            }
        }

        private void CompareEnumerable<S, D>()
        {
            var entries = Fixture.Create<TC0_I0_Members[]>();
            var sourceEntries = Mapper<TC0_I0_Members, S>.ToArray(entries);
            var destinationEntries = Mapper<S, D>.ToArray(sourceEntries);
            Assert.True(CompareEquals(sourceEntries, destinationEntries));

            sourceEntries[0] = Mapper<TC0_I0_Members, S>.Map(Fixture.Create<TC0_I0_Members>());
            Assert.False(CompareEquals(sourceEntries, destinationEntries));
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
