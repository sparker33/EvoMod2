﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatrixMath;
using System.Drawing;

namespace EvoMod2
{
	public class ResourceKernel : GaussKernel
	{
		// Global values
		public static float RESOURCESPEED;	// Movement speed for resource kernels
		public static float SPREADRATE;		// Rate at which kernels spread out over time

		// Private objects
		private Matrix moveMatrix;

		// Public objects
		public float Volume { get; set; }
		public float Smoothing { get { return H[0][0]; } }
		public Vector PositionVector
		{
			get
			{
				Vector v = new Vector(2);
				v[0] = mu[0];
				v[1] = mu[1];
				return v;
			}
		}
		public PointF Position
		{
			get
			{
				return new PointF(mu[0], mu[1]);
			}
			set
			{
				mu[0] = value.X;
				mu[1] = value.Y;
			}
		}
		public Rectangle GetBoundingBox(float widthScaling, float heightScaling)
		{
			int dia = (int)(Math.Sqrt(H[0][0]) * Volume);
			Rectangle rect = new Rectangle((int)((mu[0] - dia / 2.0f) * widthScaling),
			(int)((mu[1] - dia / 2.0f) * heightScaling),
			(int)(dia * widthScaling),
			(int)(dia * heightScaling));
			return rect;
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public ResourceKernel() : base(2)
		{
			H[0][0] = 1.0f;
			H[1][1] = 1.0f;
			H[0][1] = 0.0f;
			H[1][0] = 0.0f;
			moveMatrix = new Matrix(2, 2);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					moveMatrix[i][j] = 0.0f;
				}
			}
			Volume = 10.0f;
		}

		/// <summary>
		/// Constructor with initial volume and position specified
		/// </summary>
		/// <param name="volume"> Initial resource volume. </param>
		/// <param name="position"> Initial node center position. </param>
		public ResourceKernel(float volume, PointF position) : base(2)
		{
			Volume = volume;
			mu[0] = position.X;
			mu[1] = position.Y;
			H[0][0] = 0.01f * volume;
			H[1][1] = 0.01f * volume;
			H[0][1] = 0.0f;
			H[1][0] = 0.0f;
			moveMatrix = new Matrix(2, 2);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					moveMatrix[i][j] = RESOURCESPEED * (2.0f * (float)DisplayForm.GLOBALRANDOM.NextDouble() - 1.0f);
				}
			}
		}

		public void ZeroMoveMatrix()
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					moveMatrix[i][j] = 0.0f;
				}
			}
		}

		public float GetResourceLevelAt(PointF location)
		{
			Vector v = new Vector(2);
			v[0] = location.X;
			v[1] = location.Y;
			return (this.Volume * this.ProbabilityAt(v));
		}

		public void Update(Random random)
		{
			Vector v = new Vector(2);
			v[0] = 2.0f * (float)random.NextDouble() - 1.0f;
			v[1] = 2.0f * (float)random.NextDouble() - 1.0f;
			this.Move(v);
			if (Volume != 0.0f)
			{
				this.H = (1.0f + SPREADRATE / Volume) * this.H;
			}
			else
			{
				this.H = 1000.0f * this.H;
			}
		}

		private void Move(Vector v)
		{
			Vector dX = moveMatrix * v;
			mu[0] += dX[0];
			mu[1] += dX[1];
			if (mu[0] < 0.0f)
			{
				mu[0] = 5.0f;
				moveMatrix[0] = -1.0f * moveMatrix[0];
			}
			if (mu[0] > DisplayForm.SCALE)
			{
				mu[0] = DisplayForm.SCALE - 5.0f;
				moveMatrix[0] = -1.0f * moveMatrix[0];
			}
			if (mu[1] < 0.0f)
			{
				mu[1] = 5.0f;
				moveMatrix[1] = -1.0f * moveMatrix[1];
			}
			if (mu[1] > DisplayForm.SCALE)
			{
				mu[1] = DisplayForm.SCALE - 5.0f;
				moveMatrix[1] = -1.0f * moveMatrix[1];
			}
		}
	}
}
