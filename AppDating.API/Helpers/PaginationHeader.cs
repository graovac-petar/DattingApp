namespace AppDating.API.Helpers
{
    public class PaginationHeader
    {
        public int currentPage;
        public int itemsPerPage;
        public int totalItems;
        public int totalPages;

        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            this.currentPage = currentPage;
            this.itemsPerPage = itemsPerPage;
            this.totalItems = totalItems;
            this.totalPages = totalPages;
        }

    }
}
