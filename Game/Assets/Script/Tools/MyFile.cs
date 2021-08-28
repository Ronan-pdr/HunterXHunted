using System;
using System.Collections.Generic;

namespace Script.Tools
{
    public class MyFile<T>
    {
        // ------------ Attributs ------------
        private Node _tete;
        private Node _queue;
        
        // ------------ Constructeurs ------------
        public MyFile()
        {
            _tete = null;
            _queue = null;
        }
        
        

        public MyFile(T[] arr)
        {
            foreach (T e in arr)
            {
                Enfiler(e);
            }
        }
        
        // ------------ Getter ------------
        public bool IsEmpty() => _tete is null;

        // ------------ Méthodes ------------
        public void Enfiler(T key)
        {
            Node node = new Node(key);
            
            if (_tete is null)
            {
                _tete = node;
            }
            else
            {
                _queue.After = node;
            }
            
            _queue = node;
        }

        public T Defiler()
        {
            if (IsEmpty())
            {
                throw new Exception("You try to défiler an empty file");
            }

            T res = _tete.Key;
            _tete = _tete.After;

            if (_tete is null)
            {
                _queue = null;
            }

            return res;
        }

        public T Premier() => _tete.Key;
        
        // ------------------------ Class annexe ------------------------
        
        private class Node
        {
            // ------------ Attributs ------------
            private Node _after;
            private T _key;

            // ------------ Getters et Setter ------------
            public T Key
            {
                get => _key;
            }

            public Node After
            {
                get => _after;
                set
                {
                    if (value == this)
                        throw new Exception("You try to link a node with himself");

                    if (!(_after is null))
                        throw new Exception("Impossible to change \'After\' of a node in a file");
                
                    _after = value;
                }
            }

            // ------------ Constructeur ------------
            public Node(T key)
            {
                _key = key;
                _after = null;
            }
        }
    }
}