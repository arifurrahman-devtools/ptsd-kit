// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("NC2oPH+QapY+lPF3EdJHmJeqWn4k6VGy5QpHQoT4qFzKZ2TJ38HbQqkbmLuplJ+Qsx/RH26UmJiYnJmaASsRCsF12XfGVrYSXxFoKgl6+4MgYbEoHRZXrwfkSgC3tA0fAQ64ovJw3AjC9VxkNUMS/B2qf8+GGXaKHS3cTn03k8lOiEnQjcYFAhwWWpfXhdY9IFE51v2px0cQcAPWVAb111VW11axeiKpP2AL+ixDra8OEEot3JvwV92mpClagcBgcSQW3v6mI7QbmJaZqRuYk5sbmJiZJaRWTqvZ3DiItN8b/7axeBzx7Aij3bxPPnGOC/QW9JYD49VTqkDceXMy5Ayu50NIXYegoCIn7TIWxnFD7cC5iW+lCbpuK6BoV+I65JuamJmY");
        private static int[] order = new int[] { 6,8,6,3,4,9,11,11,13,12,13,13,12,13,14 };
        private static int key = 153;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
