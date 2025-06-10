using System.Threading.Tasks;

namespace Master.Domain.PetCare
{
    public interface IChatBot
    {
        Task<string> AskAsync(string input);
    }
}