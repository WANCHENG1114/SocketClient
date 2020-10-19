﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SocketClienTool.Code.SocketHelper
{
    public class StateObject
    {
        public Socket workSocket = null;

        public const int BufferSize = 100;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();
    }
}
