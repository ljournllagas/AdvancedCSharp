using System;
using System.Collections.Generic;
using System.Threading;

namespace AdvancedCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
           
        }

        static void UseEvents()
        {
            var video = new Video() { Title = "Video 1" };
            var videoEncoder = new VideoEncoder(); //publisher
            var mailService = new MailService(); //subscriber
            var messageService = new SmsService(); //subscriber

            videoEncoder.VideoEncoded += mailService.OnVideoEncoded;
            videoEncoder.VideoEncoded += messageService.OnVideoEncoded;

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
            Thread.Sleep(3000);

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
