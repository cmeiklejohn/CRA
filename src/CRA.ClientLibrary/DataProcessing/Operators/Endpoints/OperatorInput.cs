﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CRA.ClientLibrary.DataProcessing
{
    public class OperatorInput : IAsyncVertexInputEndpoint
    {
        protected OperatorBase _operator;
        protected int _thisId;
        private bool _isSecondaryInput = false;

        public OperatorInput(ref IVertex vertex, int thisId, bool isSecondaryInput = false)
        {
            _operator = (OperatorBase)vertex;
            _thisId = thisId;
            _isSecondaryInput = isSecondaryInput;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Console.WriteLine("Disposing OperatorInput");
            }
        }

        public async Task FromStreamAsync(Stream stream, string otherVertex, string otherEndpoint, CancellationToken token)
        {
            IEndpointContent streamEndpoint = new StreamEndpoint(stream);
            if (_isSecondaryInput)
            {
                _operator.AddSecondaryInput(_thisId, ref streamEndpoint);
                _operator.WaitForSecondaryInputCompletion(_thisId);
            }
            else
            {
                _operator.AddInput(_thisId, ref streamEndpoint);
                _operator.WaitForInputCompletion(_thisId);
            }
        }
    }
}
