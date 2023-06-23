﻿namespace ETIT.ViewModels;

public class PaginateVM<T> where T : class, new()
{
    public List<T> Data { get; set; }
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public int Take { get; set; }
    public bool HasPreview { get; set; }
    public bool HasNext { get; set; }
}
