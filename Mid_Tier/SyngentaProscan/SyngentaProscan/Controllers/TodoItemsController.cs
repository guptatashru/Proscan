using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SyngentaProscan.DataObjects;
using SyngentaProscan.Models;

namespace SyngentaProscan.Controllers
{
    public class TodoItemsController : ApiController
    {
        private MobileServiceContext db = new MobileServiceContext();

        // GET: api/TodoItems
        public IQueryable<TodoItem> GetTodoItems()
        {
            return db.TodoItems;
        }

        // GET: api/TodoItems/5
        [ResponseType(typeof(TodoItem))]
        public async Task<IHttpActionResult> GetTodoItem(string id)
        {
            TodoItem todoItem = await db.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return Ok(todoItem);
        }

        // PUT: api/TodoItems/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTodoItem(string id, TodoItem todoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            db.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TodoItems
        [ResponseType(typeof(TodoItem))]
        public async Task<IHttpActionResult> PostTodoItem(TodoItem todoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TodoItems.Add(todoItem);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TodoItemExists(todoItem.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [ResponseType(typeof(TodoItem))]
        public async Task<IHttpActionResult> DeleteTodoItem(string id)
        {
            TodoItem todoItem = await db.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            db.TodoItems.Remove(todoItem);
            await db.SaveChangesAsync();

            return Ok(todoItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TodoItemExists(string id)
        {
            return db.TodoItems.Count(e => e.Id == id) > 0;
        }
    }
}