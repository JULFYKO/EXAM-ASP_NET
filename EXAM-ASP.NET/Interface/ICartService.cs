using EXAM_ASP_NET.Data.Entities;

namespace EXAM_ASP_NET.Interfaces
{
    public interface ICartService
    {
        List<int> GetItemIds();
        List<Product> GetProducts();

        void Add(int id);
        void Clear();
        int GetCartSize();
    }
}