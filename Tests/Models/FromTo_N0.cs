using Air.Mapper;
using AutoFixture;
using AutoFixture.Kernel;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using static Air.Compare.Members;
using Emit = Air.Reflection.Emit;
using MemberInfo = Air.Reflection.MemberInfo;
using TypeInfo = Air.Reflection.TypeInfo;

namespace Internal
{
    public class FromTo_N0<S> where S : new()
    {
        private readonly ITestOutputHelper Console;
        private Fixture Fixture { get; }

        public FromTo_N0(ITestOutputHelper console)
        {
            Console = console;
            Fixture = new Fixture();
        }

        private S NewSource()
        {
            var instance = Fixture.Create<TC0_I0_Members>();

            var source = Mapper<TC0_I0_Members, S>.Map(instance);
            source = source != null ? source : new S();

            if (source != null)
                return source;

            var nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(S));
            if (nullableUnderlyingType != null)
            {
                var undelyingInstance = Activator.CreateInstance(nullableUnderlyingType);
                source = (S)Activator.CreateInstance(typeof(S), new[] { undelyingInstance });
            }

            return source;
        }

        private S NewSource(string fixtureMember, object fixtureMemberValue = null)
        {
            var instance = new TC0_I0_Members();
            var member = TypeInfo.GetMembers(typeof(TC0_I0_Members), true).First(m => m.Name == fixtureMember);

            if (member.HasSetMethod)
            {
                var setMethod = ((PropertyInfo)member.MemberTypeInfo).GetSetMethod();
                setMethod.Invoke(instance, new[] { ConvertTo(typeof(object), member.Type, fixtureMemberValue) ?? FixtureCreate(member.Type) });
            }

            var source = Mapper<TC0_I0_Members, S>.Map(instance);
            source = source != null ? source : new S();

            if (source != null)
                return source;

            var nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(S));
            if (nullableUnderlyingType != null)
            {
                var undelyingInstance = Activator.CreateInstance(nullableUnderlyingType);
                source = (S)Activator.CreateInstance(typeof(S), new[] { undelyingInstance });
            }

            return source;
        }

        private bool CanSerialize<D>(S source, D destination)
        {
            return JsonConvert.SerializeObject(source) != null &&
                JsonConvert.SerializeObject(destination) != null;
        }

        private void AssertEqualsOrDefault<D>(
            S source,
            D destination,
            bool hasReadonlyMembers) where D : new()
        {
            if (hasReadonlyMembers)
                return;

            if (hasReadonlyMembers)
                return;

            Assert.True(CanSerialize(source, destination));
            Assert.True(CompareEquals(source, destination));
        }

        private static readonly MethodInfo EnumParse = typeof(Enum).GetMethods(BindingFlags.Public | BindingFlags.Static).First(m =>
            m.IsGenericMethod &&
            m.Name == nameof(Enum.Parse) &&
            m.GetParameters().Length == 1 &&
            m.GetParameters()[0].ParameterType == typeof(string));

        private static readonly MethodInfo ObjectToString = typeof(object).GetMethod(nameof(object.ToString), new Type[] { });

        private object ConvertTo(Type source, Type destination, object value)
        {
            var method = GetConvertToMethodInfo(source, destination);
            return method?.Invoke(null, new[] { value });
        }

        private MethodInfo GetConvertToMethodInfo(
            Type nonNullableSourceType,
            Type nonNullableDestinationType)
        {
            return typeof(Convert).GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m =>
                m.Name.Contains($"To{nonNullableDestinationType.Name}") &&
                m.ReturnType == nonNullableDestinationType &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType == nonNullableSourceType);
        }

        public object FixtureCreate(Type type)
        {
            var context = new SpecimenContext(Fixture);
            return context.Resolve(type);
        }

        private bool ConvertWillThrowException(
            Type nonNullableSourceType,
            Type nonNullableDestinationType)
        {
            var source = FixtureCreate(nonNullableSourceType);
            var method = GetConvertToMethodInfo(nonNullableSourceType, nonNullableDestinationType);

            if (method == null)
                return false;

            try
            {
                method.Invoke(null, new[] { source });
                return false;
            }
            catch
            {
                return true;
            }
        }

        private Type GetUndelyingType(Type type) =>
            Nullable.GetUnderlyingType(type) ?? type;

        private void CompareConvert<D>(Type sourceType, Type destinationType) where D : new()
        {
            Random random = new Random();
            GetMembers(sourceType, destinationType, out var sourceMembers, out var destinationMembers);

            for (int s = 0; s < sourceMembers.Count; s++)
            {
                for (int d = 0; d < destinationMembers.Count; d++)
                {
                    if (ConvertWillThrowException(
                            GetUndelyingType(sourceMembers[s].Type),
                            GetUndelyingType(destinationMembers[d].Type)))
                        continue;

                    if (!Emit.ILGenerator.CanEmitLoadAndSetValue(sourceMembers[s], destinationMembers[d]))
                        continue;

                    if (GetConvertToMethodInfo(sourceMembers[s].Type, destinationMembers[d].Type) == null)
                        continue;

                    object fixtureMemberValue = null;

                    if (TypeInfo.IsNumeric(sourceMembers[s].Type) &&
                        (TypeInfo.IsNumeric(destinationMembers[d].Type) || destinationMembers[d].Type == typeof(char)))
                        fixtureMemberValue = random.Next(0, 127);

                    if (TypeInfo.IsNumeric(sourceMembers[s].Type) && TypeInfo.IsEnum(destinationMembers[d].Type))
                        fixtureMemberValue = 1;

                    if (TypeInfo.IsEnum(destinationMembers[d].Type))
                    {
                        if (sourceMembers[s].Type == typeof(string))
                            fixtureMemberValue = "B";

                        if (sourceMembers[s].Type == typeof(char))
                            fixtureMemberValue = 'B';
                    }

                    var source = NewSource(
                        sourceMembers[s].Name,
                        fixtureMemberValue ?? ConvertTo(sourceMembers[s].Type, destinationMembers[d].Type, FixtureCreate(sourceMembers[s].Type)));

                    var map = Mapper<S, D>.CompileFunc(o => o
                        .Ignore(i => i)
                        .Map(sourceMembers[s].Name, destinationMembers[d].Name));

                    var destination = map(source);

                    CompareEquals(sourceMembers[s].GetValue(source), destinationMembers[d].GetValue(destination));
                }
            }
        }

        private void GetMembers(Type sourceType, Type destinationType, out List<MemberInfo> sourceMembers, out List<MemberInfo> destinationMembers)
        {
            var outSourceMembers = TypeInfo.GetMembers(sourceType, true);
            var outDestinationMembers = TypeInfo.GetMembers(destinationType, true);

            sourceMembers = outSourceMembers.Where(s => outDestinationMembers.Exists(m => m.Name == s.Name)).OrderBy(o => o.Name).ToList();
            destinationMembers = outDestinationMembers.Where(s => outSourceMembers.Exists(m => m.Name == s.Name)).OrderBy(o => o.Name).ToList();
        }

        public void ToClass<D>(bool hasReadonlyMembers, bool hasStaticMembers) where D : new()
        {
            if (hasReadonlyMembers)
                return;

            var sourceType = typeof(S);
            var destinationType = typeof(D);

            var map = Mapper<S, D>.CompileFunc();

            S source = NewSource();
            D destination = new D();

            // =======
            destination = map(source);
            AssertEqualsOrDefault(source, destination, hasReadonlyMembers);

            CompareConvert<D>(sourceType, destinationType);
        }

        public void ToStruct<D>(bool hasReadonlyMembers, bool hasStaticMembers) where D : struct
        {
            if (hasReadonlyMembers)
                return;

            var sourceType = typeof(S);
            var destinationType = typeof(D);

            var map = Mapper<S, D>.CompileFunc();

            S source = NewSource();
            D destination = new D();

            // =======
            destination = map(source);
            AssertEqualsOrDefault(source, destination, hasReadonlyMembers);

            CompareConvert<D>(sourceType, destinationType);
        }

        public void ToNullableStruct<D>(bool hasReadonlyMembers, bool hasStaticMembers) where D : struct
        {
            if (hasReadonlyMembers)
                return;

            var sourceType = typeof(S);
            var destinationType = typeof(D?);

            GetMembers(sourceType, destinationType, out var sourceMembers, out var destinationMembers);

            var map = Mapper<S, D>.CompileFunc();

            S source = NewSource();
            D? destination = new D?();

            // =======
            destination = map(source);
            AssertEqualsOrDefault(source, destination, hasReadonlyMembers);

            CompareConvert<D?>(sourceType, destinationType);
        }
    }
}
