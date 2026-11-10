using UnityEngine;
using LombaxGuy.STG;

namespace LombaxGuy.Examples
{
    public class STG_Example : MonoBehaviour
    {
        #region Public fields
        [Header("References")]
        public Transform _player;
        public Transform _enemy;

        [Header("Settings")]
        public bool _showSequence = true;
        public bool _showGrid = true;
        public bool _showPlayer = true;
        public bool _showEnemy = true;
        #endregion

        #region Private fields
        private Vector3[][] _grid;
        #endregion

        private void OnValidate()
        {
            // create grid if it doesn't exist
            if (_grid == null)
            {
                _grid = new Vector3[10][];

                for (int y = 0; y < 10; y++)
                {
                    _grid[y] = new Vector3[10];

                    for (int x = 0; x < 10; x++)
                    {
                        _grid[y][x] = new Vector3(x, y);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            #region Sequence
            Gizmos.color = Color.black;

            TextGizmo.Draw("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz", Vector3.right * 5 + Vector3.back * 0.2f, Vector3.right * 90, 0.2f, neverCull: true);
            TextGizmo.Draw("0123456789", Vector3.right * 5 + Vector3.back * 0.5f, Vector3.right * 90, 0.2f, neverCull: true);
            #endregion

            #region Grid
            if (_showGrid)
            {
                Gizmos.color = Color.black;

                for (int y = 0; y < _grid.Length; y++)
                {
                    for (int x = 0; x < _grid[y].Length; x++)
                    {
                        TextGizmo.Draw($"({x}, {y})", new Vector3(x + .5f, 0, y + .5f), new Vector3(90, 0), 0.2f);
                    }
                }
            }
            #endregion

            #region Player
            if (_showPlayer)
            {
                Gizmos.color = Color.black;
                TextGizmo.Draw("Player", _player.position + new Vector3(0, 1.5f), 0.2f, true);

                Gizmos.color = Color.red;
                TextGizmo.Draw("HP: 2 / 10", _player.position + new Vector3(0, 1.2f), 0.2f, true);
            }
            #endregion

            #region Enemy
            if (_showEnemy)
            {
                Gizmos.color = Color.black;
                TextGizmo.Draw("Scary Enemy", _enemy.position + new Vector3(0, 1.5f), new Vector3(0, 90, 0), 0.2f, true);

                Gizmos.color = Color.green;
                TextGizmo.Draw("HP: 100 / 100", _enemy.position + new Vector3(0, 1.2f), new Vector3(0, 90, 0), 0.2f, true);
            }
            #endregion
        }
    }
}
