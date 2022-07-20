using System.Drawing;
using System.Drawing.Drawing2D;

namespace Dolores.Commands.Mocking
{
    public class MemeGenerator
    {
        public string CreateMeme(string imagePath, string topText, string bottomText)
        {
            PointF firstLocation = new PointF(10f, 10f);
            PointF secondLocation = new PointF(10f, 500f);

            Guid fileGuid = Guid.NewGuid();
            string newFilePath = $"{Path.GetTempPath()}\\{Path.GetFileNameWithoutExtension(imagePath)}-{fileGuid}.jpg";
            Bitmap bitmap = (Bitmap)Image.FromFile(imagePath);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (Font arialFont = new Font("Serif", 72, FontStyle.Bold))
                {
                    graphics.DrawString(topText, arialFont, Brushes.White, firstLocation);
                    graphics.DrawString(bottomText, arialFont, Brushes.White, secondLocation);
                }


                GraphicsPath p = new GraphicsPath();
                p.AddString(
                     topText,
                    FontFamily.GenericSansSerif,
                    (int)FontStyle.Bold,
                    graphics.DpiY * 72 / 72,
                    new Point(10, 10),
                    new StringFormat());

                p.AddString(
                    bottomText,
                   FontFamily.GenericSansSerif,
                   (int)FontStyle.Bold,
                   graphics.DpiY * 72 / 72,
                   new Point(10, 500),
                   new StringFormat());

                var pen = new Pen(Color.FromArgb(0, 0, 0), 5);
                graphics.DrawPath(pen, p);
            }

            bitmap.Save(newFilePath);

            return newFilePath;
        }

        public string CreateSpongeBob(string text)
        {
            var separatedText = SplitText(text);
            var topTextList = separatedText[0];
            var bottomTextList = separatedText[1];

            var topText = "";
            var bottomText = "";

            foreach (var word in topTextList) { topText += Sarcastify(word); }
            foreach (var word in bottomTextList) { bottomText += Sarcastify(word); }

            var memePath = CreateMeme(@"spongebob.jpg", topText, bottomText);

            return memePath;
        }

        private string Sarcastify(string word)
        {
            word = word.ToLower();
            var newWord = "";
            for (var i = 0; i < word.Length; i++)
            {
                var letter = word[i];
                if (i % 2 == 0)
                {
                    letter = Char.ToUpper(letter);
                }

                newWord += letter;
            }

            newWord += " ";

            return newWord;
        }

        private List<List<string>> SplitText(string text)
        {
            var words = text.Split(' ');
            var halfway = words.Count() / 2;
            var topText = new List<string>();
            var bottomText = new List<string>();
            var separatedWords = new List<List<string>>();

            for (var i = 0; i < words.Length; i++)
            {
                if (i <= halfway)
                {
                    topText.Add(words[i]);
                }
                else
                {
                    bottomText.Add(words[i]);
                }
            }

            separatedWords.Add(topText);
            separatedWords.Add(bottomText);

            return separatedWords;
        }

    }

}
