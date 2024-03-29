﻿using Microsoft.AspNetCore.Mvc;
using Servicios.api.Libreria.Core.Entities;
using Servicios.api.Libreria.Repository;

namespace Servicios.api.Libreria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibreriaAutorController : ControllerBase
    {
        private readonly IMongoRepository<AutorEntity> _autorGenericoRepository;

        public LibreriaAutorController(IMongoRepository<AutorEntity> autorGenericoRepository)
        {
            _autorGenericoRepository = autorGenericoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutorEntity>>> Get()
        {
            return Ok(await _autorGenericoRepository.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AutorEntity>> Get(string Id)
        {
            return Ok(await _autorGenericoRepository.GetById(Id));
        }

        [HttpPost]
        public async Task Post(AutorEntity autor)
        {
            await _autorGenericoRepository.InsertDocument(autor);
        }

        [HttpPut("{id}")]
        public async Task Put(string Id, AutorEntity autor)
        {
            autor.Id = Id;
            await _autorGenericoRepository.UpdateDocument(autor);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string Id)
        {
            await _autorGenericoRepository.DeleteDocument(Id);
        }
        [HttpPost("pagination")]
        public async Task<ActionResult<PaginationEntity<AutorEntity>>> PostPagination(PaginationEntity<AutorEntity> pagination)
        {
            //var resultados = await _autorGenericoRepository.PaginationBy(filter => filter.Nombre == pagination.Filter, pagination);

            var resultados = await _autorGenericoRepository.PaginationByFilter(pagination);

            return Ok(resultados);
        }
    }
}
