using Library.Data;
using Library.Models.Domain;
using Library.Repositories.Abstract;

namespace Library.Repositories.Implementation
{
    public class BookService : IBookService
    {
        private readonly AppDbContext context;
        public BookService(AppDbContext context)
        {
            this.context = context;
        }
        public bool Add(Book model)
        {
            try
            {
                if (model.TotalPages <= 0) { return false; }
                context.Book.Add(model);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Barrow(int id)
        {
            try
            {
                var data = context.Book.SingleOrDefault(b => b.Id == id);
                if (!data.Barrow) { data.BarrowDate = DateTime.Now; }
                else if (DateTime.Now.Day > data.BarrowDate.Day + data.MaxBarrowTime)
                {
                    Console.WriteLine(" penalties for late returns");
                }
                
                data.Barrow= ! data.Barrow;


                context.SaveChanges();
                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.FindById(id);
                if (data == null)
                    return false;
                context.Book.Remove(data);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Book FindById(int id)
        {
            return context.Book.Find(id);
        }

        public IEnumerable<Book> GetAll()
        {
            var data = (from book in context.Book
                        join author in context.Author
                        on book.AuthorId equals author.Id
                        join publisher in context.Publisher on book.PubhlisherId equals publisher.Id
                        join genre in context.Genre on book.GenreId equals genre.Id
                        select new Book
                        {
                            Id = book.Id,
                            AuthorId = book.AuthorId,
                            GenreId = book.GenreId,
                            Isbn = book.Isbn,
                            PubhlisherId = book.PubhlisherId,
                            Title = book.Title,
                            TotalPages = book.TotalPages,
                            GenreName = genre.Name,
                            AuthorName = author.AuthorName,
                            PublisherName = publisher.PublisherName
                        }
                        ).ToList();
            return data;
        }

        public bool Update(Book model)
        {
            try
            {
                context.Book.Update(model);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
