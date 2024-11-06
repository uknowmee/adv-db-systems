# [Advanced Db Systems docs](README.md) - popularity

- Some rows in [popularity_iw.csv.gz](../data/compressed/popularity_iw.csv.gz) are affected.
- We mapped unique taxonomies with popularises based on `contains()`.
    ```python
    import numpy as np
    import pandas as pd
    
    taxonomy_df = pd.read_csv('taxonomy_iw.csv', header=None, names=['parent', 'child'], escapechar='\\')
    
    unique_values = {*taxonomy_df['parent'], *taxonomy_df['child']}
    unique_values = pd.DataFrame(unique_values, columns=['name']).set_index('name')
    
    print('unique value -> popularity name')
    with open('tmp_fixed.txt', 'r') as f:
        for line in f:
            line = line.strip()
            matching_indices = unique_values.index[unique_values.index.str.contains(line)]
            for index in matching_indices:
                print(f'{index} -> {line}')
    ```
- We observed that most bad popularises are matching only once, and those who matches more are usually already existing as correct ones
  in [popularity_iw.csv.gz](../data/compressed/popularity_iw.csv.gz).
    - `,_D.C.` and `_(state)`
        - 1890s_in_Washington,_D.C. -> 1890s_in_Washington
        - 1890s_in_Washington_(state) -> 1890s_in_Washington
    - or `_Territory` and `,_D.C.`
        - 1884_in_Washington_Territory -> 1884_in_Washington
        - 1884_in_Washington,_D.C. -> 1884_in_Washington
- We decided to skip rows for which matching is not deterministic.

---

## Example affected rows:

<img src="assets/popularity-affected.png" alt="popularity-affected" width="450" />
