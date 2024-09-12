using System;
using System.Security.Authentication.ExtendedProtection;
using System.Numerics;
using System.Runtime.InteropServices;

public static class Physics2D
{
	public class PhysicsSphere
	{
		public float _radius { get; set; }
		private float _mass { get; set; }
        public Vector2 _position { get; set; }
        private Vector2 _constantForce { get; set; }
		private Vector2 _constantAcceleration { get; set; }
		private Vector2 _tempAcceleration { get; set; }
		public Vector2 _velocity { get; set; }

		public PhysicsSphere(
			float r,
			float m,
			Vector2 p,
			Vector2 f,
			Vector2 a,
			Vector2 ta,
			Vector2 v
			)
		{
			this._radius = r;
			this._mass = m;
			this._position = p;
			this._constantForce = f;
			this._constantAcceleration = a;
			this._tempAcceleration = ta;
			this._velocity = v;
		}

		// Forces that always act on the object (gravity)
        public void ApplyConstantForce()
		{
			this._acceleration = this._constantForce / this._mass;
		}

		// Temporary force
		public void ApplyForce(Vector2 force)
		{
			this._velocity += force / this._mass;
		}

		// Using acceleration to update velocity
		public void ApplyAcceleration(float dt)
		{
			this._velocity += this._acceleration * dt;
		}

		// Updating position based on current velocity
		public void ApplyVelocity(float dt)
		{
			this._position += this._velocity * dt;
		}

		public void UpdateBasicPhysics(float dt)
		{
			this.ApplyConstantForce();
			this.ApplyAcceleration(dt);
			this.ApplyVelocity(dt);
		}

		// Need to figure out direction of normal force
		public void CollideRect(CollisionRect rect)
        {
            Vector2 rectPos = rect._position;

            float distX = Math.Abs(this._position.X - rectPos.X);
            float distY = Math.Abs(this._position.Y - rectPos.Y);

			float normX;
			if (distX > 0.0) normX = distX / Math.Abs(distX);
			else normX = 0.0f;

			float normY;
			if (distY > 0.0) normY = distY / Math.Abs(distY);
			else normY = 0.0f;

            if (distX > ((rect._length / 2.0f) + this._radius)) { return; }
            if (distY > ((rect._height / 2.0f) + this._radius)) { return; }

            if (distX <= rect._length / 2.0f) {
				this._velocity = new Vector2(this._velocity.X * normX, this._velocity.Y);
			}

            if (distY <= rect._height / 2.0f) {
				this._velocity = new Vector2(this._velocity.X, this._velocity.Y * normY);
			}

            double squaredDist = Math.Pow((double)(distX - (rect._length / 2.0f)), 2.0) +
                Math.Pow((double)(distY - (rect._height / 2.0f)), 2.0);

            if (squaredDist <= (this._radius * this._radius))
			{
				if (distX < distY)
                {
                    this._velocity = new Vector2(this._velocity.X * normX, this._velocity.Y);
                }
				else
                {
                    this._velocity = new Vector2(this._velocity.X, this._velocity.Y * normY);
                }
			}
        }

		public bool IntersectRect(CollisionRect rect)
        {
            Vector2 rectPos = rect._position;

			float distX = Math.Abs(this._position.X - rectPos.X);
			float distY = Math.Abs(this._position.Y - rectPos.Y);

			if (distX > ((rect._length / 2.0f) + this._radius)) { return false; }
			if (distY > ((rect._height / 2.0f) + this._radius)) { return false; }

			if (distX <= rect._length / 2.0f) { return true; }
			if (distY <= rect._height / 2.0f) { return true; }

			double squaredDist = Math.Pow((double) (distX - (rect._length / 2.0f)), 2.0) + 
				Math.Pow((double) (distY - (rect._height / 2.0f)), 2.0);

			return (squaredDist <= (this._radius * this._radius));
        }

		public void Update(float dt)
		{
			this.UpdateBasicPhysics(dt);
		}
	}

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
