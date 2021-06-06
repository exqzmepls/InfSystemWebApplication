using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfSystemWebApplication.Models
{
    public enum PropertySpecimen
    {
        STRING,
        FOREIGN_KEY,
        QUANTITATIVE
    }

    public class EntityProperty
    {
        public PropertySpecimen Specimen { get; private set; }

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public Type Type { get; private set; }

        private EntityProperty(string name, string displayName, Type type, PropertySpecimen specimen)
        {
            Name = name;
            DisplayName = displayName;
            Type = type;
            Specimen = specimen;
        }

        public EntityProperty()
        {

        }

        public static List<EntityProperty> GetProperties(object entityType)
        {
            if (typeof(Customer).Equals(entityType) || typeof(Supplier).Equals(entityType)) return new List<EntityProperty>() { Id, OrgName, PhoneNumber };
            if (typeof(Person).Equals(entityType)) return new List<EntityProperty>() { Id, FirstName, Surname, MiddleName, PhoneNumber, DOB };
            if (typeof(Employee).Equals(entityType)) return new List<EntityProperty>() { Id, Person, Position, EntranceDate, LeaveDate };
            if (typeof(Product).Equals(entityType)) return new List<EntityProperty>() { Id, Title, Supplier, Unit, Category };
            if (typeof(Sale).Equals(entityType)) return new List<EntityProperty>() { Id, Employee, Customer, Date };
            if (typeof(ProductPrice).Equals(entityType)) return new List<EntityProperty>() { Id, Product, ValuePerOneUnit, SettingDate };
            if (typeof(SoldProduct).Equals(entityType)) return new List<EntityProperty>() { ProductPrice, Sale, Amount };
            return null;
        }

        public static readonly EntityProperty Id = new EntityProperty("Id", "Id", typeof(int), PropertySpecimen.QUANTITATIVE);

        public static readonly EntityProperty OrgName = new EntityProperty("Name", "Название организации", typeof(string), PropertySpecimen.STRING);

        public static readonly EntityProperty PhoneNumber = new EntityProperty("PhoneNumber", "Контактный телефон", typeof(string), PropertySpecimen.STRING);

        public static readonly EntityProperty FirstName = new EntityProperty("Name", "Имя", typeof(string), PropertySpecimen.STRING);

        public static readonly EntityProperty Surname = new EntityProperty("Surname", "Фамилия", typeof(string), PropertySpecimen.STRING);

        public static readonly EntityProperty MiddleName = new EntityProperty("MiddleName", "Отчество", typeof(string), PropertySpecimen.STRING);

        public static readonly EntityProperty DOB = new EntityProperty("DOB", "Дата рождения", typeof(DateTime), PropertySpecimen.QUANTITATIVE);

        public static readonly EntityProperty Person = new EntityProperty("Person", "ФИО (id)", typeof(Person), PropertySpecimen.FOREIGN_KEY);

        public static readonly EntityProperty Position = new EntityProperty("Position", "Должность", typeof(Position), PropertySpecimen.FOREIGN_KEY);

        public static readonly EntityProperty EntranceDate = new EntityProperty("EntranceDate", "Дата начала работы", typeof(DateTime), PropertySpecimen.QUANTITATIVE);

        public static readonly EntityProperty LeaveDate = new EntityProperty("LeaveDate", "Дата ухода с работы", typeof(DateTime?), PropertySpecimen.QUANTITATIVE);

        public static readonly EntityProperty Title = new EntityProperty("Name", "Название", typeof(string), PropertySpecimen.STRING);

        public static readonly EntityProperty Supplier = new EntityProperty("Supplier", "Поставщик", typeof(Supplier), PropertySpecimen.FOREIGN_KEY);

        public static readonly EntityProperty Unit = new EntityProperty("Unit", "Единица измерения", typeof(Unit), PropertySpecimen.FOREIGN_KEY);

        public static readonly EntityProperty Category = new EntityProperty("Category", "Категория", typeof(ProductCategory), PropertySpecimen.FOREIGN_KEY);

        public static readonly EntityProperty Employee = new EntityProperty("Employee", "Продавец", typeof(Employee), PropertySpecimen.FOREIGN_KEY);

        public static readonly EntityProperty Customer = new EntityProperty("Customer", "Покупатель", typeof(Customer), PropertySpecimen.FOREIGN_KEY);

        public static readonly EntityProperty Date = new EntityProperty("Date", "Дата", typeof(DateTime), PropertySpecimen.QUANTITATIVE);

        public static readonly EntityProperty Product = new EntityProperty("Product", "Товар", typeof(Product), PropertySpecimen.FOREIGN_KEY);

        public static readonly EntityProperty ValuePerOneUnit = new EntityProperty("ValuePerOneUnit", "Цена за единицу", typeof(double), PropertySpecimen.QUANTITATIVE);

        public static readonly EntityProperty SettingDate = new EntityProperty("SettingDate", "Дата назначения", typeof(DateTime), PropertySpecimen.QUANTITATIVE);

        public static readonly EntityProperty Sale = new EntityProperty("Sale", "Продажа", typeof(Sale), PropertySpecimen.FOREIGN_KEY);

        public static readonly EntityProperty ProductPrice = new EntityProperty("ProductPrice", "Товар", typeof(ProductPrice), PropertySpecimen.FOREIGN_KEY);

        public static readonly EntityProperty Amount = new EntityProperty("Amount", "Количество", typeof(double), PropertySpecimen.QUANTITATIVE);
    }
}