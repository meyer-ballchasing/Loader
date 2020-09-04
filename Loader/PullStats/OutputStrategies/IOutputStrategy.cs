using Meyer.BallChasing.Models;
using System.Threading.Tasks;

namespace Meyer.BallChasing.PullStats
{
    public interface IOutputStrategy
    {
        Task Output(Group group);
    }
}