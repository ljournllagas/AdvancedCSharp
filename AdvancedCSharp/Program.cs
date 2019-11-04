using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace AdvancedCSharp
{
    class Program
    {
        static void Main(string[] args)
        {

         

        }

        static void HandleExceptions()
        {

            try
            {
                var youtubeApi = new YoutubeApi();
                youtubeApi.GetVideos("ljourn");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                using (var streamReader = new StreamReader(@"c:\file.zip"))
                {
                    var content = streamReader.ReadToEnd();
                    Console.WriteLine(content);
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Sorry, an unexpected error occured");
            }

            try
            {
                var calculator = new Calculator();
                var result = calculator.Divide(5, 0);
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine("You cannot divide by zero");
            }
            catch (ArithmeticException ex)
            {
                Console.WriteLine("Invalid arithmetic method");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sorry, an unexpected error occured");
            }
        }
        static void UseDynamic()
        {
            dynamic a = 5;
            dynamic b = 1;
            var c = a + b; //will be dynamic

            int i = 5;
            dynamic d = i;
            long l = d;
        }
        static void UseNullableType()
        {
            DateTime? date = new DateTime(2017, 2, 3);
            DateTime date2 = date ?? DateTime.Now;

            Console.WriteLine(date2);

            Console.WriteLine("GetValueOrDefault" + date.GetValueOrDefault());
            Console.WriteLine("HasValue" + date.HasValue);
        }
        static void UseLinq()
        {
            var books = new BookRepositoryLinq().GetBooks();

            //LINQ query operators
            var cheaperBooks = from b in books
                               where b.Price < 10
                               orderby b.Title
                               select b.Title;

            //LINQ extension methods
            var cheapBooks = books
                        .Where(b => b.Price < 10)
                        .OrderBy(b => b.Title)
                        .Select(b => b.Title);

            foreach (var book in cheapBooks)
            {
                Console.WriteLine(book);
            }

            var bookSingle = books.SingleOrDefault(b => b.Title == "Title 7");

            Console.WriteLine(bookSingle?.Title);
        }
        static void UseExtensionMethods()
        {
            string post = "This is supposed to be very long blog post blah blah blah..";
            var shortedPost = post.Shorten(5);

            Console.WriteLine(shortedPost);
        }
        static void UseEvents()
        {
            var video = new Video() { Title = "Video 1" };
            var videoEncoder = new VideoEncoder(); //publisher
            var mailService = new MailService(); //subscriber
            var messageService = new SmsService(); //subscriber
            var sftp = new SFTPService();  //subscriber

            videoEncoder.VideoEncoded += mailService.OnVideoEncoded;
            videoEncoder.VideoEncoded += messageService.OnVideoEncoded;
            videoEncoder.VideoEncoded += sftp.OnVideoEncoded;

            videoEncoder.Encode(video);
        }
        static void UseLambdas()
        {
            const int factor = 5;

            Func<int, int> multiplier = n => n * factor;

            Func<string> hello = () => "Hello World";

            Console.WriteLine(multiplier(5));
            Console.WriteLine(hello.Invoke());

            var books = new BookRepository().GetBooks();

            var cheapBooks = books.FindAll(b => b.Price < 10);

            //without lambdas
            //var cheapBooks2 = books.FindAll(IsCheaperThan10Dollars);

            foreach (var book in cheapBooks)
            {
                Console.WriteLine(book.Title);
            }
        }
        static void UseDelegate()
        {
            var processor = new PhotoProcessor();
            var filters = new PhotoFilters();
            Action<Photo> filterHandler = filters.ApplyBrightness;
            filterHandler += filters.ApplyContrast;
            filterHandler += AdditionalFilters.RemoveRedEyeFilter;
            processor.Process("photo.jpg", filterHandler);
        }
        static void UseGenerics()
        {
            var number = new Nullable<int>(5);
            Console.WriteLine("Has Value " + number.HasValue);
            Console.WriteLine("Value: " + number.GetValueOrDefault());

            //null date
            var nullDate = new Nullable<DateTime>();
            Console.WriteLine("Has Value " + nullDate.HasValue);
            Console.WriteLine("Value: " + nullDate.GetValueOrDefault());
        }
    }

    #region "Using Exception Handling"
    public class Calculator
    {
        public int Divide(int numerator, int denomenator)
        {
            return numerator / denomenator;
        }
    }

    public class YoutubeApi
    {
        public List<Video> GetVideos(string user)
        {
            try
            {
                // access youtube web service
                // read the data
                // create a list of video objects
                throw new Exception("Oops some low level exception");
            }
            catch (Exception ex)
            {
                throw new YoutubeException("Could not fetch the videos from Youtube.", ex);
            }

            return new List<Video>();
        }
    }

    public class YoutubeException : Exception
    {
        public YoutubeException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
    #endregion
    #region "Using Linq"
    public class BookRepositoryLinq
    {
        public List<BookLinq> GetBooks()
        {
            return new List<BookLinq>
            {
                new BookLinq() {Title = "Title 1", Price = 5},
                new BookLinq() {Title = "Title 2", Price = 7},
                new BookLinq() {Title = "Title 3", Price = 17},
                new BookLinq() {Title = "Title 4", Price = 9},
                new BookLinq() {Title = "Title 5", Price = 99}
            };
        }
    }
    public class BookLinq
    {
        public string Title { get; set; }
        public int Price { get; set; }
    }
    #endregion
    #region "Using Extension Methods"
    public static class StringExtensions
    {
        public static string Shorten(this String str, int numberOfWords)
        {
            if (numberOfWords < 0)
                throw new ArgumentOutOfRangeException("numberOfWords should be greater than or equal to 0.");

            if (numberOfWords == 0)
                return "";

            var words = str.Split(' ');

            if (words.Length <= numberOfWords)
                return str;

            return string.Join(" ", words.Take(numberOfWords));
        }
    }
    #endregion
    #region "Using Events"
    public class Video
    {
        public string Title { get; set; }
    }

    public class VideoEventArgs : EventArgs
    {
        public Video Video { get; set; }
    }

    public class VideoEncoder
    {
        //1 - define a delegate
        //2 - define an event based on the defined delegate
        //3 - raise the event

        //1 optional
        //public delegate void VideoEncodedEventHandler(object source, VideoEventArgs args);

        //2
        public event EventHandler<VideoEventArgs> VideoEncoded;

        //default param
        //object source, eventargs e
        //public event EventHandler VideoEncoding;

        //3
        protected virtual void OnVideoEncoded(Video video)
        {
            VideoEncoded?.Invoke(this, new VideoEventArgs() { Video = video });
        }

        public void Encode(Video video)
        {
            Console.WriteLine("Encoding video..");
            //Thread.Sleep(3000);

            OnVideoEncoded(video);
        }

    }

    public class MailService
    {
        public void OnVideoEncoded(object source, VideoEventArgs e)
        {
            Console.WriteLine("MailService: Sending an email for >>> " + e.Video.Title);
        }
    }

    public class SmsService
    {
        public void OnVideoEncoded(object source, VideoEventArgs e)
        {
            Console.WriteLine("SmsService: Sending a text message for >>> " + e.Video.Title);
        }
    }


    public class SFTPService
    {
        public void OnVideoEncoded(object source, VideoEventArgs e)
        {
            Console.WriteLine("SFTP: Sending a SFTP for >>> " + e.Video.Title);
        }
    }




    #endregion
    #region "Using Lambdas"

    //static bool IsCheaperThan10Dollars(BookLambdas book)
    //{
    //    return book.Price < 10;
    //}

    public class BookRepository
    {
        public List<BookLambdas> GetBooks()
        {
            return new List<BookLambdas>
            {
                new BookLambdas() {Title = "Title 1", Price = 5},
                new BookLambdas() {Title = "Title 2", Price = 7},
                new BookLambdas() {Title = "Title 3", Price = 17}
            };
        }
    }
    public class BookLambdas
    {
        public string Title { get; set; }
        public int Price { get; set; }
    }

    #endregion
    #region "Using Delegates"
    public class AdditionalFilters
    {
        public static void RemoveRedEyeFilter(Photo photo)
        {
            Console.WriteLine("Apply remove redeye");
        }
    }

    public class PhotoFilters
    {
        public void ApplyBrightness(Photo photo)
        {
            Console.WriteLine("Apply brightness");
        }

        public void ApplyContrast(Photo photo)
        {
            Console.WriteLine("Apply contrast");
        }

        public void Resize(Photo photo)
        {
            Console.WriteLine("Resize photo");
        }
    }

    public class PhotoProcessor
    {
        public void Process(string path, Action<Photo> filterHandler)
        {
            var photo = Photo.Load(path);
            filterHandler(photo);

            photo.Save();
        }
    }

    public class Photo
    {
        public static Photo Load(string path)
        {
            return new Photo();
        }

        public void Save()
        {

        }
    }

    #endregion
    #region "Using Generics"

    // where T : struct
    public class Nullable<T> where T : struct
    {
        private object _value;

        public Nullable()
        {

        }

        public Nullable(T value)
        {
            _value = value;
        }

        public bool HasValue
        {
            get { return _value != null; }
        }

        public T GetValueOrDefault()
        {
            if (HasValue)
                return (T) _value;

            return default;
        }
    }

    // where T : Product
    // where T : class
    public class DiscountCalculator<TProduct> where TProduct : Product
    {
        public float CalculateDiscount(TProduct product)
        {
            return product.Price;
        }
    }

    public class Book : Product
    {
        public string Isbn { get; set; }
    }

    public class Product
    {
        public string Title { get; set; }
        public float Price { get; set; }
    }

    // where T : new()
    // where T : IComparable
    public class Utilities<T> where T : IComparable, new()
    {
        public int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public T Max(T a, T b) 
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        public void DoSomething(T value)
        {
            var obj = new T();
        }
    }

    public class GenericDictionary<TKey, TValue>
    {
        public void Add(TKey key, TValue value)
        {

        }
    }

    public class GenericList<T>
    {
        
        public void Add(T value)
        {

        }

        public T this[int index]
        {
            get { throw new NotImplementedException();}
        }
    }
    #endregion
}
