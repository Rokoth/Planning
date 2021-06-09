//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Planning.Contract.Model
{
    /// <summary>
    /// Базовый класс моделей
    /// </summary>
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// Идентификтаор
        /// </summary>
        [Display(Name = "Идентификатор")]
        public Guid Id { get; set; }
        /// <summary>
        /// Дата последнего изменения
        /// </summary>
        [Display(Name = "Дата последнего изменения")]
        public DateTimeOffset VersionDate { get; set; }       
    }

    public class ClientIdentityResponse
    {
        public string Token { get; set; }
        public string UserName { get; set; }
    }

    public interface IIdentity
    {
        string Login { get; set; }
        string Password { get; set; }
    }

    public class UserIdentity : IIdentity
    {
        [Display(Name = "Логин")]
        public string Login { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }

    public class User : Entity
    {
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Microsoft.AspNetCore.Mvc.Remote("CheckName", "User", ErrorMessage = "Имя уже используется")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Microsoft.AspNetCore.Mvc.Remote("CheckLogin", "User", ErrorMessage = "Логин уже используется")]
        public string Login { get; set; }
    }

    public class PagedResult<T>
    {
        public PagedResult(IEnumerable<T> data, int allCount)
        {
            Data = data;
            PageCount = allCount;
        }
        public IEnumerable<T> Data { get; }
        public int PageCount { get; }
    }
}
