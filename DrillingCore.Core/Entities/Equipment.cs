namespace DrillingCore.Core.Entities
{
    public class Equipment
    {
        public int Id { get; set; }

        // Например, Name — это название (или модель) оборудования
        public string Name { get; set; } = null!;

   

        // Регистрационный номер (номер машины и т.п.)
        public string RegistrationNumber { get; set; } = null!;

        // Дата создания записи
        public DateTime CreatedDate { get; set; }

        // Навигационное свойство на тип
        
        public int EquipmentTypeId { get; set; }

        public virtual EquipmentType? EquipmentType { get; set; }
     
    }
}
