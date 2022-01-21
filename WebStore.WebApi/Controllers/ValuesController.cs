using Microsoft.AspNetCore.Mvc;

namespace WebStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/values")]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly Dictionary<int, string> _data;

        public ValuesController(ILogger<ValuesController> logger)
        {
            var data = Enumerable.Range(1, 10)
                .Select(i => (Id: i, Value: $"Value-{i}"))
                .ToDictionary(v => v.Id, v => v.Value);
            
            _data = data;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_data.Values);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return !_data.ContainsKey(id) 
                ? NotFound() 
                : Ok(_data[id]);
        }

        [HttpGet("count")]
        //public IActionResult Count() => Ok(_Values.Count);
        //public ActionResult<int> Count() => _Values.Count;
        public int Count()
        {
            return _data.Count;
        }

        [HttpPost("add")] 
        public IActionResult Add([FromBody] string value)
        {
            var id = _data.Count == 0 ? 1 : _data.Keys.Max() + 1;
            _data[id] = value;

            return CreatedAtAction(nameof(GetById), new { id });
        }

        [HttpPut("{id:int}")]
        public IActionResult Replace(int id, [FromBody] string value)
        {
            if (!_data.ContainsKey(id))
            {
                return NotFound();
            }

            _data[id] = value;

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (!_data.ContainsKey(id))
            {
                return NotFound();
            }

            _data.Remove(id);

            return Ok();
        }
    }
}