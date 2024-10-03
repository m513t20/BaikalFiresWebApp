using System.Text.Json;

namespace SqlConfig;

public class Request{
    public string sql_request= @"select t2.""tableName"" as t1, t3.""tableName"" as t2,
       t5.name as t1FieldName,
	   t6.name as t2FieldName,
	   t2.""tableName"" as selectT1,
	   t9.""metricName"",

	   t10.group_by_t1 as is_t1,
	   t11.agrigation_type as aggr,
	   t12.name as groupby

	   
from public.links as t1
inner join public.tables as t2 on t1.""tableIdFrom"" = t2.id
inner join public.tables as t3 on t1.""tableIdTo"" = t3.id
left join public.fields as t5 on t5.id::text = fieldIdTo
left join public.fields as t6 on t6.id::text = fieldFrom
inner join public.metricbridge as t7 on t7.tableId = t2.id  
inner join public.metricbridge as t8 on t8.tableId = t3.id  
inner join public.metrics as t9 on t9.id = t7.metricId

inner join public.agrigationbridge as t10 on t10.metric_id=t9.id
inner join public.agrigation as t11 on t11.id=t10.agrigation_id
inner join public.fields as t12 on t12.id=t11.agrigation_field

where t7.metricId = {0} and t8.metricId = {0}";
	public string format_request=@"
Select  t1_gr@.groupby@, aggr@ from  
public.t2@
inner join public.t1@ on t2@.t2fieldname@ = t1@.t1fieldname@ 
group by  t1_gr@.groupby@";
	public string connection=string.Empty;
    public string connection_study=string.Empty;
	public Request(){
        var file_settings="constants/config.json";
        var settings_info = File.ReadAllText(file_settings);
        var settings_str = JsonSerializer.Deserialize<Settings>(settings_info);
		connection=settings_str.ConnectionString;
        connection_study=settings_str.ConnectionStudy;
	
}
}
record Settings(string ConnectionString, string ConnectionStudy);