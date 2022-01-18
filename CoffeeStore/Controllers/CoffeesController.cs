using AutoMapper;
using CoffeeStore.Dtos;
using CoffeeStore.Models;
using CoffeeStore.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeesController : ControllerBase
    {
        private readonly ICoffeeRepo _repository;
        private readonly IMapper _mapper;

        public CoffeesController(ICoffeeRepo repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        ///<summary>
        ///Fetches all Coffees in the DB that are not currently flagged for checkout
        ///</summary>
        ///<returns>A List of CoffeeReadDto</returns>
        [HttpGet]
        public ActionResult<IEnumerable<CoffeeReadDto>> GetCoffees()
        {
            var coffees = _repository.GetCoffees();
            return Ok(_mapper.Map<IEnumerable<CoffeeReadDto>>(coffees));
        }

        ///<summary>
        ///Fetches a single coffee from the DB with a specific id 
        ///</summary>
        ///<param name="id"></param>
        /// <returns>A single CoffeeReadDto</returns>
        [HttpGet("{id}", Name = " GetCoffeeById")]
        public ActionResult<CoffeeReadDto> GetCoffeeById(int id)
        {
            var coffee = _repository.GetCoffeeById(id);
            if(coffee == null)
                return NotFound();
            
            return Ok(_mapper.Map<CoffeeReadDto>(coffee));
        }

        ///<summary>
        ///Inserts a Coffee into the db 
        ///</summary>
        ///<param name="coffeeCreateDto"></param>
        /// <returns>The created Coffee as a CoffeeReadDto</returns>
        [HttpPost]
        public ActionResult<CoffeeCreateDto> CreateCoffee([FromBody] CoffeeCreateDto coffeeCreateDto)
        {
            var coffeeModel = _mapper.Map<Coffee>(coffeeCreateDto);

            _repository.CreateCoffee(coffeeModel);

            var coffeeReadDto = _mapper.Map<CoffeeReadDto>(coffeeModel);

            return CreatedAtAction(nameof(GetCoffeeById), new { id = coffeeReadDto.Id }, coffeeReadDto);
        }

        ///<summary>
        ///Toggles a Coffee in the DB for checkout 
        ///</summary>
        ///<param name="id"></param>
        /// <returns>The updated Coffee as a CoffeeReadDto</returns>
        [HttpPut]
        public ActionResult UpdateCoffeeForCheckout(int id)
        {
            var coffee = _repository.UpdateCoffeeForCheckout(id);
            if (coffee == null)
                return NotFound();

            return Ok(_mapper.Map<CoffeeReadDto>(coffee));
        }

        ///<summary>
        ///Deletes a Coffee in the DB
        ///For this Coding Challenge 'deleted' means the item has been sold
        ///</summary>
        ///<param name="id"></param>
        /// <response code="204">If the deletion was successful</response>
        /// <response code="404">If the coffee was not found in the DB</response>
        [HttpDelete]
        public ActionResult DeleteCoffeeById(int id)
        {
            var coffee = _repository.GetCoffeeById(id);
            if (coffee == null)
                return NotFound();

            _repository.DeleteCoffee(id);

            return NoContent();
        }
    }
}
