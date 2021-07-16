using System.Threading.Tasks;

namespace jhaCC.Services.Reporting
{
    // auto report interface defining single method to RenderReport()
    public interface IAutoReport
    {
        Task RenderReport();
    }
}