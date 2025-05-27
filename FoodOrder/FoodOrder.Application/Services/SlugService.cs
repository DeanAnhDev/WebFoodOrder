using FoodOrder.Domain.Interfaces;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FoodOrder.Application.Services
{
    public class SlugService
    {
        private readonly ISlugRepository _slugRepository;

        public SlugService(ISlugRepository slugRepository)
        {
            _slugRepository = slugRepository;
        }

        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            string slug = stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLower();

            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');

            return slug;
        }

        public async Task<string> GenerateUniqueSlug<T>(string input) where T : class
        {
            string baseSlug = GenerateSlug(input);
            string slug = baseSlug;
            int counter = 1;

            while (await _slugRepository.SlugExistsAsync<T>(slug))
            {
                slug = $"{baseSlug}-{counter++}";
            }

            return slug;
        }

    }
}
