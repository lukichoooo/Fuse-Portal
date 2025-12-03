namespace InfrastructureTests
{
    public static class MapperTestHelper
    {
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


        public static void AssertCommonPropsByName<A, B>(A a, B b)
        {
            var propsA = typeof(A).GetProperties().Select(p => p.Name);
            var propsB = typeof(B).GetProperties().Select(p => p.Name);

            foreach (var propName in propsA.Intersect(propsB))
            {
                var propA = typeof(A).GetProperty(propName)!;
                var valA = propA.GetValue(a);
                var valB = typeof(B).GetProperty(propName)!.GetValue(b);

                if (valA != null && IsComplexType(propA.PropertyType))
                {
                    AssertCommonPropsByName(valA, valB);
                }
                else
                {
                    Assert.That(valA, Is.EqualTo(valB), $"Mismatch on property {propName}");
                }
            }
        }

    }
}
