﻿using Bogus;
using CashFlow.Domain.Entities;
using CommonTesteUtilities.Cryptography;

namespace CommonTesteUtilities.UserBuilder
{
    public class UserBuilder
    {
        public static User Build()
        {
            var passwordEncrypter = new PasswordEncrypterBuilder().Build();

            var user = new Faker<User>()
                .RuleFor(u => u.Id, _ => 1)
                .RuleFor(u => u.Name, faker => faker.Person.FirstName)
                .RuleFor(u => u.Email, (faker, user) => faker.Internet.Email(user.Name))
                .RuleFor(u => u.Password, (_, user) => passwordEncrypter.Encrypt(user.Password))
                .RuleFor(u => u.User_identifier, _ => Guid.NewGuid());

            return user;
        }
    }
}
