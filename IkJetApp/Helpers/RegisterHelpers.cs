namespace IkJetApp.Helpers
{
    public class RegisterHelpers
    {

        public static string CreateEmailAddress(string firstName, string lastName, string companyName)
        {
            firstName = firstName.ToLower();
            lastName = lastName.ToLower();
            companyName = companyName.ToLower();

            firstName = ConvertTurkishCharacters(firstName);
            lastName = ConvertTurkishCharacters(lastName);
            companyName = ConvertTurkishCharacters(companyName);

            companyName = companyName.Replace(" ", string.Empty);

            string email = $"{firstName}.{lastName}@{companyName}.com";
            return email;
        }
        public static string ConvertTurkishCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            input = input.Replace('ı', 'i');
            input = input.Replace('İ', 'I');
            input = input.Replace('ç', 'c');
            input = input.Replace('Ç', 'C');
            input = input.Replace('ö', 'o');
            input = input.Replace('Ö', 'O');
            input = input.Replace('ü', 'u');
            input = input.Replace('Ü', 'U');
            input = input.Replace('ğ', 'g');
            input = input.Replace('Ğ', 'G');
            input = input.Replace('ş', 's');
            input = input.Replace('Ş', 'S');
            return input;
        }

        public static bool TCValidation(string TCIDentityNumber)
        {// TC Kimlik Numarası 11 haneli olmalıdır.
            if (TCIDentityNumber.Length != 11)
            {
                return false;
            }

            // TC Kimlik Numarası sadece rakamlardan oluşmalıdır.
            foreach (char c in TCIDentityNumber)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            // İlk hane 0 olamaz.
            if (TCIDentityNumber[0] == '0')
            {
                return false;
            }

            // İlk 10 hane üzerinden kontrol işlemi yapılır.
            int[] digits = new int[11];
            for (int i = 0; i < 11; i++)
            {
                digits[i] = int.Parse(TCIDentityNumber[i].ToString());
            }

            int oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
            int evenSum = digits[1] + digits[3] + digits[5] + digits[7];

            int tenthDigit = (oddSum * 7 - evenSum) % 10;
            int eleventhDigit = (oddSum + evenSum + digits[9]) % 10;

            return digits[9] == tenthDigit && digits[10] == eleventhDigit;


           
        }

        public static string GeneratePassword()
        {
            string guid = Guid.NewGuid().ToString("N").Substring(0, 9);

            if (!ContainsUppercase(guid))
            {
                guid = AddUppercase(guid);
            }

            if (!ContainsDigit(guid))
            {
                guid = AddDigit(guid);
            }

            if (!ContainsSpecialCharacter(guid))
            {
                guid = AddSpecialCharacter(guid);
            }

            return guid;
        }

        static bool ContainsUppercase(string input)
        {
            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    return true;
                }
            }
            return false;
        }

        static bool ContainsDigit(string input)
        {
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    return true;
                }
            }
            return false;
        }

        static bool ContainsSpecialCharacter(string input)
        {
            foreach (char c in input)
            {
                if (c == '+' || c == '-' || c == '*' || c == '.')
                {
                    return true;
                }
            }
            return false;
        }

        static string AddUppercase(string input)
        {
            if (ContainsUppercase(input))
            {
                return input;
            }


            char[] chars = input.ToCharArray();
            Random random = new Random();


            int index = random.Next(chars.Length);


            char uppercaseLetter = (char)random.Next('A', 'Z' + 1);

            chars[index] = uppercaseLetter;

            return new string(chars);
        }

        static string AddDigit(string input)
        {
            char[] chars = input.ToCharArray();
            Random random = new Random();

            chars[random.Next(chars.Length)] = (char)random.Next('0', '9' + 1);
            return new string(chars);
        }

        static string AddSpecialCharacter(string input)
        {
            char[] chars = input.ToCharArray();
            Random random = new Random();

            char[] specialChars = { '+', '-', '*', '.' };
            chars[random.Next(chars.Length)] = specialChars[random.Next(specialChars.Length)];
            return new string(chars);
        }



        public static bool IsPasswordValid(string password)
        {
            if (password.Length < 9)
                return false;

            bool hasUppercase = false;
            bool hasLowercase = false;
            bool hasDigit = false;
            bool hasSpecialCharacter = false;

            char[] specialChars = { '+', '-', '*', '.', '/', ':', '=', '|', '@', '#', '$', '%', '^', '&', '(', ')', '[', ']', '{', '}', '<', '>', '?' };

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUppercase = true;
                if (char.IsLower(c))
                    hasLowercase = true;
                if (char.IsDigit(c))
                    hasDigit = true;
                if (Array.Exists(specialChars, ch => ch == c))
                    hasSpecialCharacter = true;
            }

            return hasUppercase && hasLowercase && hasDigit && hasSpecialCharacter;
        }











    }
}
