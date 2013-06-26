using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameOfLife_Model
{
    public class Cell
    {
        private const string CHR_ALIVE = " A ";
        private const string CHR_DEAD = " · ";

        public const bool ALIVE = true;
        public const bool DEAD = false;

        public bool State = DEAD;

        public Cell()
        {
            this.State = DEAD;
        }

        public Cell(bool state)
        {
            this.State = state;
        }

        public bool IsAlive()
        {
            return (this.State == ALIVE);
        }

        public bool IsDead()
        {
            return (this.State == DEAD);
        }

        public override string ToString()
        {
            return (IsAlive() ? CHR_ALIVE : CHR_DEAD);
        }

        public void SetState(bool newState)
        {
            this.State = newState;
        }

        public void ToggleState()
        {
            this.State = !this.State;
        }

        public void SetAlive()
        {
            this.State = ALIVE;
        }

        public void SetDead()
        {
            this.State = DEAD;
        }
    }
}