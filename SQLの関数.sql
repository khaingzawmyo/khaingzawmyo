SELECT 
    name,
    -- 1. 大文字に変換
    UPPER(name) AS upper_name,
    -- 2. 文字数をカウント
    LENGTH(name) AS name_length,
    -- 3. 文字列の一部を切り出す（nameの1文字目から4文字分）
    SUBSTR(name, 1, 4) AS short_name,
    -- 4. 前後の無駄な空白を削除する
    TRIM(department_code) AS clean_dept,
    -- 5. 文字列を置換する（.com を .co.jp に変更）
    REPLACE(email, '.com', '.co.jp') AS new_email
FROM employees;