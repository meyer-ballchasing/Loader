using Meyer.BallChasing.Models;
using System.Threading.Tasks;

namespace Meyer.BallChasing.PullStats
{
    public interface IOutputStrategy
    {
        Task OutputGameSummary(Group group);

        Task OutputGroupSummary(Group group);

        Task OutputSummaryAcrossGroups(Group group);
    }
}