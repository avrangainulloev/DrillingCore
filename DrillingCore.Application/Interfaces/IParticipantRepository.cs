using DrillingCore.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrillingCore.Application.Interfaces
{
    public interface IParticipantRepository
    {


        // Получение участника по его идентификатору
        Task<Participant?> GetByIdAsync(int id);

        // Получение списка участников для заданного проекта
        Task<IEnumerable<Participant>> GetByProjectIdAsync(int projectId);

        // Добавление нового участника в проект
        Task AddAsync(Participant participant);

        // Обновление данных участника (например, при завершении участия)
        Task UpdateAsync(Participant participant);

        // Удаление участника
        Task DeleteAsync(Participant participant);
    }
}
