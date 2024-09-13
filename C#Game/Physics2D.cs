using System;
using System.Security.Authentication.ExtendedProtection;
using System.Numerics;
using System.Runtime.InteropServices;
/*
 * Stephen Gavin Sears
 * Professor Aline Normoyle
 * Game Programming
 * September 12, 2024
 * 
 * Very Awful 2D "physics" engine I wrote that supports dynamic circles 
 * and rectangles it can collide with
*/

public static class Physics2D
{
	public class PhysicsSphere
	{
		public float _radius { get; set; }
		private float _mass { get; set; }
        public Vector2 _position { get; set; }
        private Vector2 _constantForce { get; set; }
		public Vector2 _velocity { get; set; }

		public PhysicsSphere(
			float r,
			float m,
			Vector2 p,
			Vector2 f,
			Vector2 v
			)
		{
			this._radius = r;
			this._mass = m;
			this._position = p;
			this._constantForce = f;
			this._velocity = v;
		}

		// Forces that always act on the object (gravity)
        public void ApplyConstantForce(float dt)
		{
			this._velocity += (this._constantForce / this._mass) * dt;
		}

		// Applies 1 second's worth of given force onto sphere
		public void ApplyForce(Vector2 force)
		{
			this._velocity += (force / this._mass);
		}

		// Give us a maximum velocity so we do not phase through walls
		public void CapVelocity()
		{
			if (Math.Abs(this._velocity.X) > 80.0)
            {
                this._velocity = new Vector2(
						79.0f * this._velocity.X / Math.Abs(this._velocity.X),
						this._velocity.Y
					);
            }
			if (Math.Abs(this._velocity.Y) > 145.0)
			{
				this._velocity = new Vector2(
						this._velocity.X, 
						144.0f * this._velocity.Y / Math.Abs(this._velocity.Y)
					);
			}
        }

		// Updating position based on current velocity
		public void ApplyVelocity(float dt)
		{
			this.CapVelocity();
			this._position += this._velocity * dt;
		}

		// Apply constant forces, and apply velocity
		public void UpdateBasicPhysics(float dt)
		{
			this.ApplyConstantForce(dt);
			this.ApplyVelocity(dt);
		}

		// Checks multiple rects for collision updates
		public void CollideRects(CollisionRect[] rects)
		{
			foreach (CollisionRect rect in rects)
			{
				this.CollideRect(rect);
			}
		}

		// Check for collision from rect and update
		// velocity accordingly
		public void CollideRect(CollisionRect rect)
        {
            Vector2 rectPos = rect._position;

			// Absolute x and y distances from the rect
            double distX = Math.Abs(
					this._position.X - (rectPos.X + (rect._length / 2.0))
				);

            double distY = Math.Abs(
					this._position.Y - (rectPos.Y + (rect._height / 2.0))
				);

			// normalized distances (1 or -1 so we can solve later issues)
            double normX;
			if (distX > 0.0) 
				normX = (this._position.X - (rectPos.X + (rect._length / 2.0))) / distX;
			else normX = 0.0;
			// Mapped to 0 and 1 for later use
			double flippedX = ((normX + 1.0) / 2.0);

			// same as previous block
            double normY;
			if (distY > 0.0) 
				normY = (this._position.Y - (rectPos.Y + (rect._height / 2.0))) / distY;
			else normY = 0.0;
			double flippedY = ((normY + 1.0) / 2.0);

			// End code early if ball is too far away
            if (distX > ((rect._length / 2.0) + (this._radius * 2 * -(flippedX - 1.0)))) { return; }
            if (distY > ((rect._height / 2.0) + (this._radius * 2 * -(flippedY - 1.0)))) { return; }

			// Checking we are within the x bounds if we want to do a vertical bounce
			if ((this._position.X <= (rect._position.X + rect._length) - this._radius - 4) 
				&& (this._position.X >= rect._position.X + 4)
				)
            {
                // flipping vertical velocity and snapping position
                double sideChange = (flippedY * (rect._height - (this._radius * 2)));
				this._position = new Vector2(
						this._position.X, 
						(float)((rectPos.Y + ((this._radius) * 2 * normY)) + sideChange)
					);
				this._velocity = new Vector2(this._velocity.X, -this._velocity.Y);
				return;
            }
			// Checking we are within the y bounds if we want to do a horizontal bounce
			if ((this._position.Y <= (rect._position.Y + rect._height) - 4)
                && (this._position.Y >= rect._position.Y + 4)
                )
            {
                // flipping horizontal velocity and snapping position
                double sideChange = (flippedX * (rect._length - (this._radius * 2)));
                this._position = new Vector2(
						(float)((rectPos.X + ((this._radius) * 2 * normX)) + sideChange), 
						this._position.Y
					);
                this._velocity = new Vector2(-this._velocity.X, this._velocity.Y);
				return;
            }


			// If collision is on corner, we check here
			// A little glitchy, but it does not seem worth fixing at the moment
            double squaredDist = Math.Pow((double)(distX - (rect._length / 2.0f)), 2.0) +
                Math.Pow((double)(distY - (rect._height / 2.0f)), 2.0);

            if (squaredDist - (this._radius * this._radius) * 4 <= (this._radius * this._radius))
			{
				if (distY - (rect._height / 2.0f) < distX - (rect._length / 2.0f))
                {
					// flipping vertical velocity and snapping position
                    double sideChange = (flippedY * (rect._height - (this._radius * 2)));
                    this._position = new Vector2(
                            this._position.X,
                            (float)((rectPos.Y + ((this._radius) * 2 * normY)) + sideChange)
                        );
                    this._velocity = new Vector2(this._velocity.X, -this._velocity.Y);
                }
				else
                {
                    // flipping horizontal velocity and snapping position
                    double sideChange = (flippedX * (rect._length - (this._radius * 2)));
                    this._position = new Vector2(
                            (float)((rectPos.X + ((this._radius) * 2 * normX)) + sideChange),
                            this._position.Y
                        );
                    this._velocity = new Vector2(-this._velocity.X, this._velocity.Y);
                }
            }
        }

		// Tells me if circle is colliding with rect
		// (should be tweaked for better accuracy like CollideRect)
		public bool IntersectRect(CollisionRect rect)
        {
            Vector2 rectPos = rect._position;

            // Absolute x and y distances from the rect
            double distX = Math.Abs(
                    this._position.X - (rectPos.X + (rect._length / 2.0))
                );

            double distY = Math.Abs(
                    this._position.Y - (rectPos.Y + (rect._height / 2.0))
                );

            // normalized distances (1 or -1 so we can solve later issues)
            double normX;
            if (distX > 0.0)
                normX = (this._position.X - (rectPos.X + (rect._length / 2.0))) / distX;
            else normX = 0.0;
            // Mapped to 0 and 1 for later use
            double flippedX = ((normX + 1.0) / 2.0);

            // same as previous block
            double normY;
            if (distY > 0.0)
                normY = (this._position.Y - (rectPos.Y + (rect._height / 2.0))) / distY;
            else normY = 0.0;
            double flippedY = ((normY + 1.0) / 2.0);

            // End code early if ball is too far away
            if (distX > ((rect._length / 2.0) + (this._radius * 2 * -(flippedX - 1.0)))) { return false; }
            if (distY > ((rect._height / 2.0) + (this._radius * -(flippedY - 2)))) { return false; }

            // Checking we are within the x bounds if we want to do a vertical bounce
            if ((this._position.X <= (rect._position.X + rect._length) - this._radius - 1)
                && (this._position.X >= rect._position.X + 1)
                )
            {
                return true;
            }
            // Checking we are within the y bounds if we want to do a horizontal bounce
            if ((this._position.Y <= (rect._position.Y + rect._height) - 1)
                && (this._position.Y >= rect._position.Y + 1)
                )
            {
                return true;
            }


            // If collision is on corner, we check here
            // A little glitchy, but it does not seem worth fixing at the moment
            double squaredDist = Math.Pow((double)(distX - (rect._length / 2.0f)), 2.0) +
                Math.Pow((double)(distY - (rect._height / 2.0f)), 2.0);

            if (squaredDist <= (this._radius * this._radius))
            {
                return true;
            }

            return false;
        }

		public void Update(float dt)
		{
			this.UpdateBasicPhysics(dt);
		}
	}

	/*
	 * Simple Collision box
	 */ 
	public class CollisionRect
	{
		public float _length { get; set; }
		public float _height { get; set; }

        public Vector2 _position { get; set; }

        public float _left { get; set; }
        public float _right { get; set; }
        public float _top { get; set; }
        public float _bottom { get; set; }

        // Top left, top right,
        // bottom left, bottom right
        private Vector2[] _corners;

		public CollisionRect(float l, float h, Vector2 pos)
		{
			this._length = l;
			this._height = h;
			this._position = pos;

			this._left = pos.X - (l / 2.0f);
            this._right = pos.X + (l / 2.0f);
            this._top = pos.Y - (h / 2.0f);
            this._bottom = pos.Y + (h / 2.0f);


            this._corners = [
					new Vector2(_left, _top),
                    new Vector2(_right, _top),
                    new Vector2(_left, _bottom),
                    new Vector2(_right, _bottom)
				];
		}
	}

    /*
	 * I have used code from online for this struct
	 * because it did not make sense to code this
	 * myself. Code based on stack overflow user
	 * https://stackoverflow.com/users/2125720/jayram
	 * post: https://stackoverflow.com/questions/17249919/c-sharp-vector2-code
	 */
    public struct Vector2
	{
		public float X;
		public float Y;

		public Vector2(float x, float y)
		{
			this.X = x;
			this.Y = y;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator *(Vector2 v1, float m)
        {
            return new Vector2(v1.X * m, v1.Y * m);
        }

        public static Vector2 operator /(Vector2 v1, float m)
        {
            return new Vector2(v1.X / m, v1.Y / m);
        }

        public static float Distance(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public override string ToString()
        {
			return "{ " + this.X + ", " + this.Y + " }";
        }
    }
}
