using AutoFixture;
using Models;
using Statics.Compare;
using Statics.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Statics.Compare.Members;

namespace Playground.Tests
{
    [Collection(nameof(Playground))]
    public class Types
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

        public Types()
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
        public void FromToEnum()
        {
            TS0_I1_Nullable_Members left = new();
            TS0_I1_Nullable_Members right = new();
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

        [Fact]
        public void MemberDiffs()
        {
            var entries = Fixture.Create<TC_0[]>();
            var leftEntries = Mapper<TC_0[], TC_0[]>.Map(entries);
            var rightEntries = Mapper<TC_0[], TC_0[]>.Map(leftEntries);
            Assert.True(CompareEquals(leftEntries, rightEntries));

            leftEntries[0].N2.N1.Members[0] = Mapper<TC0_I0_Members, TC0_I0_Members>.Map(Fixture.Create<TC0_I0_Members>());

            Assert.False(CompareEquals(leftEntries, rightEntries, out IEnumerable<MemberDiff> memberDiffs, evaluateChildNodes: true));
            Assert.NotEmpty(memberDiffs);
        }
    }
}
