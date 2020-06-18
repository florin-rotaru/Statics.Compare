using Air.Mapper;
using AutoFixture;
using Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static Air.Compare.Members;

namespace Playground
{
    public class Recursive
    {
        private readonly ITestOutputHelper Console;

        private Fixture Fixture { get; }

        public Recursive(ITestOutputHelper console)
        {
            Console = console;
            Fixture = new Fixture();
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(Fixture);
        }

        //TRecursiveNode GetRecursiveNode(int recursion)
        //{
        //    return new TRecursiveNode
        //    {
        //        Name = Fixture.Create<string>(),
        //        ChildNodes = recursion > 0 ? new List<TRecursiveNode>
        //            {
        //                GetRecursiveNode(recursion - 1),
        //                GetRecursiveNode(recursion - 1),
        //                GetRecursiveNode(recursion - 1)
        //            } : new List<TRecursiveNode>(),
        //        ParentNode = recursion > 0 ? new TRecursiveNode
        //        {
        //            ChildNodes = recursion > 0 ? new List<TRecursiveNode>
        //                {
        //                    GetRecursiveNode(recursion - 1),
        //                    GetRecursiveNode(recursion - 1),
        //                    GetRecursiveNode(recursion - 1)
        //                } : new List<TRecursiveNode>(),
        //            ParentNode = recursion > 0 ? GetRecursiveNode(recursion - 1) : new TRecursiveNode()
        //        } : new TRecursiveNode()
        //    };
        //}

        //[Fact]
        //public void Nodes()
        //{
        //    var source = GetRecursiveNode(2);

        //    var map = Mapper<TRecursiveNode, TRecursiveNode>.CompileFunc();
        //    var destination = map(source);

        //    Assert.True(CompareEquals(source, destination));

        //    map = Mapper<TRecursiveNode, TRecursiveNode>.CompileFunc(o => o
        //        .Map(s => s.ParentNode, d => d.ParentNode));
        //    destination = map(source);

        //    Assert.True(CompareEquals(source, destination));
        //}
    }
}
