namespace Products.Tests.Integrations.Helpers;

public static class HttpHelper
{
    public static string GetProducts(int page) => "/api/products/" + page;
    public static string GetProductById(string guid) => "/api/products/" + guid;
    public static string DeleteProductById(string guid) => "/api/products/" + guid;
    public static string CreateProduct() => "/api/products/";
}