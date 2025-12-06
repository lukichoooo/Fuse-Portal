using System.Collections;

namespace InfrastructureTests
{
    public static class HelperMapperTest
    {
        private const int MAX_RECURSION_DEPTH = 3;


        public static void AssertCommonPropsByName<A, B>(A a, B b, int depth = 0)
        {
            if (depth == MAX_RECURSION_DEPTH) return;

            var propsA = typeof(A).GetProperties().Select(p => p.Name);
            var propsB = typeof(B).GetProperties().Select(p => p.Name);

            foreach (var propName in propsA.Intersect(propsB))
            {
                var propA = typeof(A).GetProperty(propName)!;
                var valA = propA.GetValue(a);
                var valB = typeof(B).GetProperty(propName)!.GetValue(b);

                if (valA != null && IsComplexType(propA.PropertyType))
                {
                    AssertCommonPropsByName(valA, valB, depth + 1);
                }
                else if (IsEnumerable(propA.PropertyType))
                {
                    var listA = ((IEnumerable)valA).Cast<object>().ToList();
                    var listB = ((IEnumerable)valB).Cast<object>().ToList();

                    Assert.That(listA.Count, Is.EqualTo(listB.Count));

                    for (int i = 0; i < listA.Count; i++)
                        AssertCommonPropsByName(listA[i], listB[i], depth + 1);
                }
                else
                {
                    Assert.That(valA, Is.EqualTo(valB), $"Mismatch on property {propName}");
                }
            }
        }


        public static bool IsEnumerable(Type t)
        {
            if (t == typeof(string))
                return false;

            return typeof(IEnumerable).IsAssignableFrom(t);
        }


        public static bool IsComplexType(Type t)
        {
            if (t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(DateTime) || t == typeof(Guid))
            {
                return false;
            }

            var assemblyName = t.Assembly.GetName().Name;
            if (assemblyName != null &&
                (assemblyName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) ||
                 assemblyName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            return t.IsClass || t.IsValueType;
        }


    }
}
