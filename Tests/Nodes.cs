using Air.Reflection;
using Xunit;
using static Test.Models;

namespace Test
{
    public class Nodes
    {
        [Fact]
        public void List()
        {
            var nodes = TypeInfo.GetNodes<Node>(true);

            Assert.True(nodes.Exists(w => w.Name == nameof(Node.Segment)));
            Assert.True(nodes.Exists(w => w.Name == nameof(Node.Segment) + "." + nameof(Node.Segment.SystemTypeCodes)));

            nodes = TypeInfo.GetNodes<StructSegment>(true);

            Assert.True(nodes.Exists(w => w.Name == nameof(StructSegment.SystemTypeCodes)));

            nodes = TypeInfo.GetNodes<StructSegment?>(true);

            Assert.True(nodes.Exists(w => w.Name == nameof(StructSegment.SystemTypeCodes)));

            nodes = TypeInfo.GetNodes<StructSegment?>(false);

            Assert.False(nodes.Exists(w => w.Name == nameof(StructSegment.SystemTypeCodes)));
        }

        [Fact]
        public void Name()
        {
            var member = nameof(Node.Segment) + "." + nameof(Node.Segment.SystemTypeCodes) + "." + nameof(Node.Segment.SystemTypeCodes.StringType);

            Assert.Equal(nameof(Node.Segment) + "." + nameof(Node.Segment.SystemTypeCodes), TypeInfo.GetNodeName(member));
        }

        [Fact]
        public void Recursion()
        {
            var recursion_0 = TypeInfo.GetNodes(typeof(RecursiveNode), true);

            Assert.NotEmpty(recursion_0);

            var recursion_1 = TypeInfo.GetNodes(typeof(RecursiveNode), true, 1);

            Assert.True(recursion_1.Count > recursion_0.Count);
        }
    }
}
