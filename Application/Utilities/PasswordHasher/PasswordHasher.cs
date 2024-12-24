using System.Security.Cryptography;

namespace Application.Utilities.PasswordHasher;

public static class PasswordHasher
{
    // تابع برای هش کردن پسورد با PBKDF2 و Salt
    public static string HashPassword(string password)
    {
        // ایجاد Salt
        byte[] salt = GenerateSalt();

        // تعداد تکرار (iterations) بالا برای افزایش امنیت
        int iterations = 10000;

        // استفاده از PBKDF2 برای هش کردن پسورد
        byte[] hash = PBKDF2_HashPassword(password, salt, iterations);

        // تبدیل به رشته Base64
        string hashString = Convert.ToBase64String(hash);
        string saltString = Convert.ToBase64String(salt);

        // بازگشت به صورت رشته‌ای شامل هش و Salt
        return $"{iterations}:{saltString}:{hashString}";
    }

    // تابع برای تولید Salt تصادفی
    private static byte[] GenerateSalt()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] salt = new byte[32];  // طول Salt برابر با 32 بایت
            rng.GetBytes(salt);
            return salt;
        }
    }

    // تابع برای هش کردن پسورد با PBKDF2
    private static byte[] PBKDF2_HashPassword(string password, byte[] salt, int iterations)
    {
        using (var hmac = new HMACSHA256(salt))
        {
            // محاسبه هش پسورد با PBKDF2
            var derivedKey = new byte[32]; // طول هش 32 بایت است
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                derivedKey = pbkdf2.GetBytes(32); // دریافت 32 بایت از هش
            }
            return derivedKey;
        }
    }

    // تابع برای اعتبارسنجی پسورد وارد شده با هش ذخیره‌شده
    public static bool VerifyPassword(string enteredPassword, string storedHash)
    {
        // جدا کردن پارامترها از رشته ذخیره‌شده
        string[] parts = storedHash.Split(':');
        int iterations = int.Parse(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] storedHashBytes = Convert.FromBase64String(parts[2]);

        // تولید هش از پسورد وارد شده
        byte[] hash = PBKDF2_HashPassword(enteredPassword, salt, iterations);

        // مقایسه هش تولیدشده با هش ذخیره‌شده
        return hash.SequenceEqual(storedHashBytes);
    }

}