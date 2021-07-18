using System;

namespace FlatFiles
{
    internal interface IRecoverableRecordContext : IRecordContext
    {
        event EventHandler<ColumnErrorEventArgs>? ColumnError;

        bool HasHandler { get; }

        void ProcessError(object sender, ColumnErrorEventArgs e);
    }
}
