﻿using System;
using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class QueryableExtensionsTests
    {
        [Test]
        public void InlinePropertyWithoutAttribute()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameWithoutAttribute.Computed() == "Test User"
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlineProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullName == "Test User"
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void ConcatNonStringInlineProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User", From = 0, To = 100 } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.From + "-" + employee.To) == "0-100"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FromTo == "0-100"
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlinePropertyOrderBy()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            orderby (employee.FirstName + " " + employee.LastName)
                            select employee);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where employee.FullName == "Test User"
                          orderby employee.FullName
                          select employee);

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlinePropertyOrderByThenBy()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            orderby (employee.FirstName + " " + employee.LastName)
                            select employee).ThenBy(x => true);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where employee.FullName == "Test User"
                          orderby employee.FullName 
                          select employee).ThenBy(x => x.IsActive);

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlineBooleanProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where true
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.IsActive
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void TestLdflda()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                where employee.Reference.Count == 0
                select employee);

            var actual = (from employee in employees.AsQueryable()
                where employee.Count == 0
                select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlineTooDeepProperty()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.TooDeepName == "Test User"
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlinePropertyWithVariableClosure()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            string fullname = "Test User";
            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == fullname
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullName == fullname
                          select employee).Decompile();

            Console.WriteLine(expected);
            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlineMethod()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameMethod() == "Test User"
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlineMethodWithArg()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where ("Mr " + employee.FirstName + " " + employee.LastName) == "Mr Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameMethod("Mr ") == "Mr Test User"
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlineMethodWithTwoArgs()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where ("Mr " + employee.FirstName + " " + employee.LastName + " Jr.") == "Mr Test User Jr."
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullNameMethod("Mr ", " Jr.") == "Mr Test User Jr."
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test, Ignore("Minor differences")]
        public void Issue39()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.NullableDate.HasValue && (employee.NullableInt.HasValue && employee.NullableDate.Value.AddDays(employee.NullableInt.Value) > DateTime.Now))
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.Test
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void Issue58()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = employees.AsQueryable().Where(_ => (_.Id <= 3 ? (_.Id > 3 ? 3 : 2) : 1) == 1);

            var actual = employees.AsQueryable().Where(_ => _.ComplexProperty == 1).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlineExtensionMethod()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            select employee);

            var actual = (from employee in employees.AsQueryable()
                          where employee.FullName().Computed() == "Test User"
                          select employee).Decompile();

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlineExtensionMethodOrderBy()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            orderby (employee.FirstName + " " + employee.LastName)
                            select employee);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where employee.FullName().Computed() == "Test User"
                          orderby employee.FullName ().Computed()
                          select employee);

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlineExtensionMethodOrderByThenBy()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.FirstName + " " + employee.LastName) == "Test User"
                            orderby (employee.FirstName + " " + employee.LastName)
                            select employee).ThenBy(x => true);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where employee.FullName().Computed() == "Test User"
                          orderby employee.FullName().Computed()
                          select employee).ThenBy(x => x.IsActive);

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlinePropertyNullableShortColeasce1()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.MyField.HasValue ? employee.MyField.Value : (short)0) > 0
                            select employee);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where employee.TheBad > (short)0
                          select employee);

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

        [Test]
        public void InlinePropertyNullableShortColeasce2()
        {
            var employees = new[] { new Employee { FirstName = "Test", LastName = "User" } };

            var expected = (from employee in employees.AsQueryable()
                            where (employee.MyField.HasValue ? (short)0 : (short)1) > (short)0
                            select employee);

            var actual = (from employee in employees.AsQueryable().Decompile()
                          where (employee.MyField.HasValue ? (short)0 : (short)1) > 0
                          select employee);

            Assert.AreEqual(expected.Expression.ToString(), actual.Expression.ToString());
        }

    }
}
