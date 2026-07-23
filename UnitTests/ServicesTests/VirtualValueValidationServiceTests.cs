#nullable enable
using NUnit.Framework;
using PxGraf.Models.Queries;
using PxGraf.Services;
using System.Collections.Generic;

namespace UnitTests.ServicesTests
{
    [TestFixture]
    internal class VirtualValueValidationServiceTests
    {
        private VirtualValueValidationService _service = new();

        private static List<string> RealCodes(params string[] codes) => [.. codes];

        // ---- Valid cases ----

        [Test]
        public void Validate_ValidSum2Operands_NoErrors()
        {
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["a", "b"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b"), out _);

            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Validate_ValidSum3Operands_NoErrors()
        {
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["a", "b", "c"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b", "c"), out _);

            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Validate_ValidSubtractionTwoOperands_NoErrors()
        {
            List<VirtualValueDefinition> defs =
            [
                new SubtractionOfTwoDefinition { Code = "v1", Minuend = "a", Subtrahend = "b" }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b"), out _);

            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Validate_ValidSubtractionOneOperandPlusConstant_NoErrors()
        {
            List<VirtualValueDefinition> defs =
            [
                new SubtractionByConstantDefinition { Code = "v1", Operand = "a", Constant = 5.0 }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b"), out _);

            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Validate_ValidMultiplicationTwoOperands_NoErrors()
        {
            List<VirtualValueDefinition> defs =
            [
                new MultiplicationOfTwoDefinition { Code = "v1", LeftOperand = "a", RightOperand = "b" }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b"), out _);

            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Validate_ValidMultiplicationOneOperandPlusConstant_NoErrors()
        {
            List<VirtualValueDefinition> defs =
            [
                new MultiplicationByConstantDefinition { Code = "v1", Operand = "a", Constant = 3.0 }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a"), out _);

            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Validate_ValidDivisionTwoOperands_NoErrors()
        {
            List<VirtualValueDefinition> defs =
            [
                new DivisionOfTwoDefinition { Code = "v1", Dividend = "a", Divisor = "b" }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b"), out _);

            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Validate_ValidDivisionOneOperandPlusConstant_NoErrors()
        {
            List<VirtualValueDefinition> defs =
            [
                new DivisionByConstantDefinition { Code = "v1", Operand = "a", Constant = 2.0 }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a"), out _);

            Assert.That(isValid, Is.True);
        }

        // ---- Code validation ----

        [Test]
        public void Validate_NullCode_ReturnsError()
        {
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = null, OperandCodes = ["a", "b"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b"), out _);

            Assert.That(isValid, Is.False);
        }

        [Test]
        public void Validate_EmptyCode_ReturnsError()
        {
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "", OperandCodes = ["a", "b"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b"), out _);

            Assert.That(isValid, Is.False);
        }

        [Test]
        public void Validate_DuplicateVirtualCodes_ReturnsError()
        {
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["a", "b"] },
                new SumDefinition { Code = "v1", OperandCodes = ["a", "b"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b"), out string? error);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(isValid, Is.False);
                Assert.That(error, Does.Contain("Duplicate"));
            }
        }

        [Test]
        public void Validate_VirtualCodeCollidesWithRealCode_ReturnsError()
        {
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "a", OperandCodes = ["b", "c"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b", "c"), out string? error);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(isValid, Is.False);
                Assert.That(error, Does.Contain("collides"));
            }
        }

        // ---- Operand reference validation ----

        [Test]
        public void Validate_OperandReferencingNonExistentRealValue_ReturnsError()
        {
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["a", "nonexistent"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a"), out string? error);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(isValid, Is.False);
                Assert.That(error, Does.Contain("nonexistent"));
            }
        }

        [Test]
        public void Validate_OperandReferencingNonExistentVirtualValue_ReturnsError()
        {
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["a", "v99"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a"), out string? error);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(isValid, Is.False);
                Assert.That(error, Does.Contain("v99"));
            }
        }

        // ---- Forward / chained references ----

        [Test]
        public void Validate_ChainWithoutCycle_SingleVirtualDependsOnReal_NoErrors()
        {
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["a", "b"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b"), out _);

            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Validate_ChainedVirtualDefinitionsInOrder_NoErrors()
        {
            // vB defined first (depends on real a,b), then vA depends on virtual vB
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "vB", OperandCodes = ["a", "b"] },
                new SumDefinition { Code = "vA", OperandCodes = ["vB", "c"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b", "c"), out _);

            Assert.That(isValid, Is.True);
        }

        // ---- Cycle detection ----

        [Test]
        public void Validate_DirectCircularDependency_SelfReference_ReturnsError()
        {
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["v1", "a"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a"), out string? error);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(isValid, Is.False);
                Assert.That(error, Does.Contain("Circular"));
            }
        }

        [Test]
        public void Validate_IndirectCircularDependency_TwoNodes_ReturnsError()
        {
            // v1 depends on v2, v2 depends on v1
            List<VirtualValueDefinition> defs =
            [
                new SumDefinition { Code = "v1", OperandCodes = ["v2", "a"] },
                new SumDefinition { Code = "v2", OperandCodes = ["v1", "b"] }
            ];

            bool isValid = _service.Validate(defs, RealCodes("a", "b"), out string? error);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(isValid, Is.False);
                Assert.That(error, Does.Contain("Circular"));
            }
        }
    }
}
#nullable disable