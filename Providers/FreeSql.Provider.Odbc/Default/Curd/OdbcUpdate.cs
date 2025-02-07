﻿using FreeSql.Internal;
using FreeSql.Internal.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSql.Odbc.Default
{

    class OdbcUpdate<T1> : Internal.CommonProvider.UpdateProvider<T1>
    {
        OdbcUtils _utils;
        public OdbcUpdate(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere)
            : base(orm, commonUtils, commonExpression, dywhere)
        {
            _utils = _commonUtils as OdbcUtils;
        }

        public override int ExecuteAffrows() => base.SplitExecuteAffrows(_batchRowsLimit > 0 ? _batchRowsLimit : _utils.Adapter.UpdateBatchSplitLimit, _batchParameterLimit > 0 ? _batchParameterLimit : 255);
        public override List<T1> ExecuteUpdated() => base.SplitExecuteUpdated(_batchRowsLimit > 0 ? _batchRowsLimit : _utils.Adapter.UpdateBatchSplitLimit, _batchParameterLimit > 0 ? _batchParameterLimit : 255);

        protected override List<T1> RawExecuteUpdated() => throw new NotImplementedException("FreeSql.Odbc.Default 未实现该功能");

        protected override void ToSqlCase(StringBuilder caseWhen, ColumnInfo[] primarys)
        {
            if (_table.Primarys.Length == 1)
            {
                var pk = _table.Primarys.First();
                caseWhen.Append(_commonUtils.QuoteReadColumn(pk.CsType, pk.Attribute.MapType, _commonUtils.QuoteSqlName(pk.Attribute.Name)));
                return;
            }
            caseWhen.Append("(");
            var pkidx = 0;
            foreach (var pk in _table.Primarys)
            {
                if (pkidx > 0) caseWhen.Append(" + '+' + ");
                caseWhen.Append(_utils.Adapter.CastSql(_commonUtils.QuoteReadColumn(pk.CsType, pk.Attribute.MapType, _commonUtils.QuoteSqlName(pk.Attribute.Name)), _utils.Adapter.MappingOdbcTypeVarChar));
                ++pkidx;
            }
            caseWhen.Append(")");
        }

        protected override void ToSqlWhen(StringBuilder sb, ColumnInfo[] primarys, object d)
        {
            if (_table.Primarys.Length == 1)
            {
                sb.Append(_commonUtils.FormatSql("{0}", _table.Primarys[0].GetDbValue(d)));
                return;
            }
            var pkidx = 0;
            foreach (var pk in _table.Primarys)
            {
                if (pkidx > 0) sb.Append(" + '+' + ");
                sb.Append(_utils.Adapter.CastSql(_commonUtils.FormatSql("{0}", pk.GetDbValue(d)), _utils.Adapter.MappingOdbcTypeVarChar));
                ++pkidx;
            }
        }

#if net40
#else
        public override Task<int> ExecuteAffrowsAsync() => base.SplitExecuteAffrowsAsync(_batchRowsLimit > 0 ? _batchRowsLimit : _utils.Adapter.UpdateBatchSplitLimit, _batchParameterLimit > 0 ? _batchParameterLimit : 255);
        public override Task<List<T1>> ExecuteUpdatedAsync() => base.SplitExecuteUpdatedAsync(_batchRowsLimit > 0 ? _batchRowsLimit : _utils.Adapter.UpdateBatchSplitLimit, _batchParameterLimit > 0 ? _batchParameterLimit : 255);

        protected override Task<List<T1>> RawExecuteUpdatedAsync() => throw new NotImplementedException("FreeSql.Odbc.Default 未实现该功能");
#endif
    }
}
